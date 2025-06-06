export class Part {    
    constructor(id: string,
                name: string,
                description: string,
                level : number,
                leaderIds: string[]) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.level = level;
        this.leaderIds = leaderIds;
    }

    id?: string;

    name?: string;
    description?: string;
    level?: number;

    mainPartId?: string = "00000000-0000-0000-0000-000000000000";
    typeId? : number;
    leaderIds?: string[];
    parts?: Part[];
}