export class Employee {

    constructor(id : string, firstName : string, lastName : string, patronymic : string) {
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.patronymic = patronymic;
    }

    id? : string;
    department_id?: string;

    firstName? : string;
    lastName? : string;
    patronymic? : string;
}