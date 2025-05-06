export class TaskStatus {    
    constructor(id: string,
                name: string,
                description: string,
                order : number,
                partId: string) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.order = order;
        this.partId = partId;
    }

    id?: string;

    name?: string;
    description?: string;
    order?: number;
    isFixed?: boolean;
    globalStatus?: number;
    accessLevel?: number;
    partId: string = "00000000-0000-0000-0000-000000000000";

}