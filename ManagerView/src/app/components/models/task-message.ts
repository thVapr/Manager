import { Member } from "./member";

export interface TaskMessage {
    id? : string;
    message : string;
    creatorId : string;
    taskId : string;
    creator? : Member;
    createdAt?: Date;
}