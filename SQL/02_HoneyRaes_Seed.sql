\c HoneyRaes

INSERT INTO Customer (Name, Address) VALUES ('Janet Hollander', '123 Bender Ln');
INSERT INTO Customer (Name, Address) VALUES ('Thomas Yertz', '456 Tantamount Place');
INSERT INTO Customer (Name, Address) VALUES ('Verna Ulrich', '789 Grandstand Blvd');

INSERT INTO Employee (Name, Specialty) VALUES ('Rosie Dorfmeister', 'Screen Repair');
INSERT INTO Employee (Name, Specialty) VALUES ('Brenda Lu', 'Debugging out');
INSERT INTO Employee (Name, Specialty) VALUES ('Frank Gertz', 'Water cooler talk');

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) 
VALUES (1, 1, 'Cracked Screen', false, '2023-11-24');

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency)
VALUES (2, NULL, 'Blue screen of death', true);

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency)
VALUES (3, 1, 'Feelings of despair', true);

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted)
VALUES (1, 2, 'Button moves slowly', false, '2023-11-26');

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency)
VALUES (2, NULL, 'Doesn''t charge', false);

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency)
VALUES (2, 1, 'Gum on screen', false);