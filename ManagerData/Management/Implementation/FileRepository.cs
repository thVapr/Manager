using ManagerData.Constants;
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace ManagerData.Management.Implementation;

public class FileRepository : IFileRepository
{
    private readonly MinioClient _minio;
    private readonly string _bucket;
    private readonly MainDbContext _database;
    private readonly ILogger<FileRepository> _logger;
    
    public FileRepository(MainDbContext database, IConfiguration config, ILogger<FileRepository> logger)
    {
        var secretProvider = new SecretProvider();
        var settings = config.GetSection("Minio");
        _database = database;
        _logger = logger;

        _bucket = settings["Bucket"];
        _minio = (MinioClient?)new MinioClient()
            .WithEndpoint(settings["Endpoint"])
            .WithCredentials(secretProvider.GetStorageAccessKey(), secretProvider.GetStorageSecretKey())
            .WithSSL(bool.Parse(settings["UseSSL"]))
            .Build();
    }

    public async Task<List<TaskFile>> ListFilesAsync(Guid taskId)
    {
        var objects = new List<string>();

        var args = new ListObjectsArgs()
            .WithBucket("files")
            .WithRecursive(true);

        var observable = _minio.ListObjectsEnumAsync(args);
        await foreach (var item in observable)
        {
            objects.Add(item.Key);
        }

        var fileNames = await _database.TaskFiles
            .Where(file => objects.Any(obj => obj == file.Path) && file.TaskId == taskId)
            .ToListAsync();
        return fileNames;
    }
    
    public async Task UploadFileAsync(Guid taskId, string objectName, Stream data, string contentType)
    {
        try
        {
            var pathToFile = Guid.NewGuid() + "." + Path.GetExtension(objectName).TrimStart('.').ToLowerInvariant();
            await _database.TaskFiles.AddAsync(new TaskFile
            {
                TaskId = taskId,
                FileName = objectName,
                Path = pathToFile,
            });
            await _database.SaveChangesAsync();
            var bucketExists = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket));
            if (!bucketExists)
            {
                await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket));
            }

            await _minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucket)
                .WithObject(pathToFile)
                .WithStreamData(data)
                .WithObjectSize(data.Length)
                .WithContentType(contentType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[{DateTime.Now}]");
            return;
        }

    }

    public async Task<Stream> GetFileAsync(string objectName, Guid taskId)
    {
        try
        {
            var ms = new MemoryStream();

            var file = (await _database.TaskFiles
                .FirstOrDefaultAsync(file => file.TaskId == taskId && file.FileName == objectName));

            if (file == null)
                return ms;

            var tcs = new TaskCompletionSource();

            await _minio.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(file.Path)
                .WithCallbackStream(async stream =>
                {
                    await stream.CopyToAsync(ms);
                    ms.Position = 0;
                    tcs.SetResult();
                }));

            await tcs.Task;
            return ms;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[{DateTime.Now}]");
            return new MemoryStream();
        }
    }

    public async Task RemoveFileAsync(string objectName, Guid taskId)
    {
        try
        {
            var file = (await _database.TaskFiles
                .FirstOrDefaultAsync(file => file.TaskId == taskId && file.FileName == objectName));
            if (file == null)
                return;
            await _minio.RemoveObjectAsync(new  RemoveObjectArgs()
                .WithBucket(_bucket)
                .WithObject(file.Path)
            );
            _database.TaskFiles.Remove(file);
            await _database.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[{DateTime.Now}]");
            return;
        }
    }
}