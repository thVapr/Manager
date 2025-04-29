export class Member {

    constructor(id : string, firstName : string, lastName : string, patronymic : string) {
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.patronymic = patronymic;
    }

    id? : string;
    
    departmentId?: string;
    departmentName? : string;

    companyId?: string;
    companyName? : string;

    projectId?: string;
    projectName?: string;

    firstName? : string;
    lastName? : string;
    patronymic? : string;
}