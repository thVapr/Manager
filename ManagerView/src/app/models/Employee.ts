export class Employee {

    constructor(userId : string, firstName : string, lastName : string, patronymic : string) {
        this.userId = userId;
        this.firstName = firstName;
        this.lastName = lastName;
        this.patronymic = patronymic;
    }

    userId? : string;
    department_id?: string;

    firstName? : string;
    lastName? : string;
    patronymic? : string;
}