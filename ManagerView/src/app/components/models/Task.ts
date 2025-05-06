export class Task {
    id?: string;

    name?: string;
    description?: string;
    
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