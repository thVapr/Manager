export class Task {
    id?: string;

    name?: string;
    description?: string;
    path? : string;
    
    memberId?: string;
    memberName?: string;
    memberLastName? : string;

    creatorId?: string;
    partId?: string;

    startTime? : Date;
    deadline? : Date;
    closedAt? : Date;

    status?: number;
    level?: number;
}