export class Member {

    constructor(id : string, firstName : string, lastName : string, patronymic : string) {
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.patronymic = patronymic;
    }

    id? : string;

    partId?: string;
    partName?: string;
    privilege?  : number;

    firstName? : string;
    lastName? : string;
    patronymic? : string;
    
    messengerId? : string;
    isMessengerConfirmed? : boolean;
}