import { Member } from "./member";

export interface TaskHistory {    
    taskId : string;
    sourceStatusId : number;
    destinationStatusId : number;
    initiatorId : string;
    targetMemberId : string;
    actionType : TaskActionType;
    createdAt : Date;
    description : string;
    name : string;
    initiator? : Member;
    targetMember? : Member;
}

export enum TaskActionType {
    Created = 0,
    StatusChanged,
    Assigned,
    Reassigned,
    MemberAdded,
    Commented,
    Renamed,
    DescriptionChanged,
    DeadlineChanged,
    Deadline,
    FileAdded,
    FileRemoved
}

export enum TaskActionHeader {
    Created = "Создание задачи",
    StatusChanged = "Статус изменен",
    Assigned = "Задача прикреплена",
    Reassigned = "Задача откреплена",
    MemberAdded = "Исполнитель добавлен",
    Commented = "Комментарий добавлен",
    Renamed = "Переименование задачи",
    DescriptionChanged = "Обновление описания",
    DeadlineChanged = "Смена крайнего срока",
    Deadline = "Наступил крайний срок",
    FileAdded = "Добавлен файл",
    FileRemoved = "Удалён файл",
}