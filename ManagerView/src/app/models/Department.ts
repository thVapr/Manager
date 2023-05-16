export class Department {    
    constructor(id: string,
                name: string,
                description: string,
                managerId: string) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.managerId = managerId;
    }

    id?: string;

    name?: string;
    description?: string;
    
    companyId?: string;
    managerId?: string
}