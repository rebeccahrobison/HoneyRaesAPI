using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using HoneyRaesAPI.Models;
using HoneyRaesAPI.Models.DTOs;


// Not needed since data models are using the namespace
// List<HoneyRaesAPI.Models.Customer> customers = new List<HoneyRaesAPI.Models.Customer> {};
// List<HoneyRaesAPI.Models.Employee> employees = new List<HoneyRaesAPI.Models.Employee> {};
// List<HoneyRaesAPI.Models.ServiceTicket> serviceTickets = new List<HoneyRaesAPI.Models.ServiceTicket> {};

List<Customer> customers = new List<Customer>
{
    new Customer()
    {
        Id = 1,
        Name = "Janet Hollander",
        Address = "123 Bender Ln"
    },
    new Customer
    {
        Id = 2,
        Name = "Frank Gertz",
        Address = "456 Tantamount Place"
    },
    new Customer
    {
        Id = 3,
        Name = "Verna Ulrich",
        Address = "789 Grandstand Blvd"
    }
};
List<Employee> employees = new List<Employee>
{
    new Employee()
    {
        Id = 1,
        Name = "Rosie Dorfmeister",
        Specialty = "Screen Repair"
    },
    new Employee()
    {
        Id = 2,
        Name = "Brenda Lu",
        Specialty = "Debugging out"
    },
    new Employee()
    {
        Id = 3,
        Name = "Frank Gertz",
        Specialty = "Water cooler talk"
    }
};
List<ServiceTicket> serviceTickets = new List<ServiceTicket>
{
    new ServiceTicket()
    {
        Id = 1,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "Cracked Screen",
        Emergency = false,
        DateCompleted = new DateTime(2023, 11, 24)
    },
    new ServiceTicket()
    {
        Id = 2,
        CustomerId = 2,
        Description = "Blue screen of death",
        Emergency = true,
    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 3,
        EmployeeId = 1,
        Description = "Feelings of despair",
        Emergency = true,
    },
    new ServiceTicket()
    {
        Id = 4,
        CustomerId = 1,
        EmployeeId = 2,
        Description = "Button moves slowly",
        Emergency = false,
        DateCompleted = new DateTime(2023, 11, 26)
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 2,
        Description = "Doesn't charge",
        Emergency = false,
    },
    new ServiceTicket()
    {
        Id = 6,
        CustomerId = 2,
        Description = "Gum on screen",
        Emergency = false,
        EmployeeId = 1
    }
 };



var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapGet("/servicetickets", () =>
{
    return serviceTickets.Select(t => new ServiceTicketDTO
    {
        Id = t.Id,
        CustomerId = t.CustomerId,
        EmployeeId = t.EmployeeId,
        Description = t.Description,
        Emergency = t.Emergency,
        DateCompleted = t.DateCompleted
    });
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);
    Employee employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    return Results.Ok(new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        Customer = customer == null ? null : new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        EmployeeId = serviceTicket.EmployeeId,
        Employee = employee == null ? null : new EmployeeDTO
        {
            Id = employee.Id,
            Name = employee.Name,
            Specialty = employee.Specialty
        },
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency,
        DateCompleted = serviceTicket.DateCompleted
    });
});

app.MapGet("/employees", () =>
{
    return employees.Select(e => new EmployeeDTO
    {
        Id = e.Id,
        Name = e.Name,
        Specialty = e.Specialty
    });
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    List<ServiceTicket> tickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(new EmployeeDTO
    {
        Id = employee.Id,
        Name = employee.Name,
        Specialty = employee.Specialty,
        ServiceTickets = tickets.Select(t => new ServiceTicketDTO
        {
            Id = t.Id,
            CustomerId = t.CustomerId,
            EmployeeId = t.EmployeeId,
            Description = t.Description,
            Emergency = t.Emergency,
            DateCompleted = t.DateCompleted
        }).ToList()
    });
});

app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    List<ServiceTicket> tickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    return Results.Ok(new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address,
        ServiceTickets = tickets.Select(t => new ServiceTicketDTO
        {
            Id = t.Id,
            CustomerId = t.CustomerId,
            EmployeeId = t.EmployeeId,
            Description = t.Description,
            Emergency = t.Emergency,
            DateCompleted = t.DateCompleted
        }).ToList()
    });
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (SQL will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    // serviceTickets.Add(serviceTicket);

    // Get the customer data to check that the customerid for the service ticket is valid
    Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);

    // if the client did not provide a valid customer id, this is a bad request
    if (customer == null)
    {
        return Results.BadRequest();
    }

    serviceTickets.Add(serviceTicket);

    // Created returns a 201 status code with a link in the headers to where the new resource can be accessed
    return Results.Created($"/servicetickets/{serviceTicket.Id}", new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        Customer = new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency
    });

});

app.MapDelete("servicetickets/{id}", (int id) =>
{
    ServiceTicket ticketToDelete = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (ticketToDelete == null)
    {
        return Results.NotFound();
    }
    serviceTickets.Remove(ticketToDelete);

    return Results.NoContent();
});

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }

    ticketToUpdate.CustomerId = serviceTicket.CustomerId;
    ticketToUpdate.EmployeeId = serviceTicket.EmployeeId;
    ticketToUpdate.Description = serviceTicket.Description;
    ticketToUpdate.Emergency = serviceTicket.Emergency;
    ticketToUpdate.DateCompleted = serviceTicket.DateCompleted;

    return Results.NoContent();
});

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);

    ticketToComplete.DateCompleted = DateTime.Today;
});

// Return all service tickets that are incomplete and emergencies
app.MapGet("/servicetickets/emergencies", () =>
{
    List<ServiceTicket> foundTickets = serviceTickets
        .Where(st => st.Emergency)
        .Where(st => st.DateCompleted == DateTime.MinValue)
        .ToList();

    return foundTickets.Select(ft => new ServiceTicketDTO
    {
        Id = ft.Id,
        CustomerId = ft.CustomerId,
        EmployeeId = ft.EmployeeId,
        Description = ft.Description,
        Emergency = ft.Emergency,
        DateCompleted = ft.DateCompleted
    });
});

// Return all unassigned service tickets
app.MapGet("/servicetickets/unassigned", () =>
{
    List<ServiceTicket> foundTickets = serviceTickets.Where(st => st.EmployeeId == null).ToList();

    return foundTickets.Select(ft => new ServiceTicketDTO
    {
        Id = ft.Id,
        CustomerId = ft.CustomerId,
        EmployeeId = ft.EmployeeId,
        Description = ft.Description,
        Emergency = ft.Emergency,
        DateCompleted = ft.DateCompleted
    });
});

// Return customers that haven't had a tickets closed in over a year
app.MapGet("/servicetickets/inactivecustomers", () =>
{
    DateTime aYearAgo = DateTime.Today.AddYears(-1);
    List<ServiceTicket> foundTickets = serviceTickets.Where(st => st.DateCompleted <= aYearAgo).ToList();
    List<Customer> foundCustomers = customers.Where(c => foundTickets.Any(t => c.Id == t.CustomerId)).ToList();

    return foundCustomers.Select(fc => new CustomerDTO
    {
        Id = fc.Id,
        Name = fc.Name
    });
});

// Return employees not currently assigned to an incomplete service ticket
app.MapGet("/employees/unassigned", () =>
{
    List<ServiceTicket> uncompletedTickets = serviceTickets.Where(st => st.DateCompleted == DateTime.MinValue).ToList();
    foreach (ServiceTicket t in uncompletedTickets)
    {
        Console.WriteLine(t.Id);
    }
    List<Employee> foundEmployees = employees
        .Where(e => uncompletedTickets.Any(ut => e.Id != ut.EmployeeId && ut.EmployeeId != null))
        .ToList();

    return foundEmployees.Select(fe => new EmployeeDTO
    {
        Id = fe.Id,
        Name = fe.Name,
        Specialty = fe.Specialty
    });
});

// Return all customers for whom a given employee has been assigned to a service ticket
app.MapGet("/employees/{id}/customers", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    List<ServiceTicket> employeeTickets = serviceTickets.Where(st => st.EmployeeId == employee.Id).ToList();
    List<Customer> foundCustomers = customers.Where(c => employeeTickets.Any(et => et.CustomerId == c.Id)).ToList();

    // Fancier way to find the customers of an employee
    // List<Customer> foundCustomers = serviceTickets
    // .Where(st => st.EmployeeId == employee.Id)
    // .Join(customers, et => et.CustomerId, c => c.Id, (et, c) => c)
    // .ToList();

    return Results.Ok(foundCustomers.Select(fc => new CustomerDTO
    {
        Id = fc.Id,
        Name = fc.Name
    }));
});

// Return employee with most completed service tickets
app.MapGet("/employees/mostticketscompleted", () =>
{
    DateTime aMonthAgo = DateTime.Today.AddMonths(-1);

    // Employee employeeWithMostTickets = employees
    //     .OrderByDescending(e => serviceTickets
    //         .Count(st => st.EmployeeId == e.Id && st.DateCompleted >= aMonthAgo))
    //     .FirstOrDefault();

    Employee employeeWithMostTickets = null;
    int maxTicketCount = 0;

    foreach (Employee emp in employees)
    {
        int Count = 0;
        foreach (ServiceTicket st in serviceTickets)
        {
            if (st.EmployeeId == emp.Id && st.DateCompleted >= aMonthAgo)
            {
                Count++;
            }
        }
        if (Count > maxTicketCount)
        {
            maxTicketCount = Count;
            employeeWithMostTickets = emp;
        }
    }
    if (employeeWithMostTickets != null)
    {
        return Results.Ok(
        new EmployeeDTO
        {
            Id = employeeWithMostTickets.Id,
            Name = employeeWithMostTickets.Name,
            Specialty = employeeWithMostTickets.Specialty
        }
        );

    }
    else
    {
        return Results.NotFound();
    }
});

// Return completed tickets from oldest to newest
app.MapGet("/servicetickets/bydatecompleted", () =>
{
    List<ServiceTicket> completedTickets = serviceTickets.Where(st => st.DateCompleted > DateTime.MinValue).ToList();
    List<ServiceTicket> serviceTicketsByDate = completedTickets.OrderByDescending(st => st.DateCompleted).ToList();
    return serviceTicketsByDate.Select(t => new ServiceTicketDTO
    {
        Id = t.Id,
        CustomerId = t.CustomerId,
        EmployeeId = t.EmployeeId,
        Description = t.Description,
        Emergency = t.Emergency,
        DateCompleted = t.DateCompleted
    });
});

// Return incomplete tickets by if emergent, then unassigned, then assigned
app.MapGet("/servicetickets/prioritized", () =>
{
    List<ServiceTicket> incompleteTickets = serviceTickets.Where(st => st.DateCompleted == DateTime.MinValue).ToList();
     List<ServiceTicket> prioritizedTickets = incompleteTickets
        .OrderByDescending(it => it.Emergency)
        .ThenBy(it => it.EmployeeId == null) //Order by if ticket has no employee assigned
        .ThenBy(it => it.EmployeeId) // Order by if ticket has an employee assigned
        .ToList();

    return prioritizedTickets.Select(t => new ServiceTicketDTO
    {
        Id = t.Id,
        CustomerId = t.CustomerId,
        EmployeeId = t.EmployeeId,
        Description = t.Description,
        Emergency = t.Emergency,
        DateCompleted = t.DateCompleted
    });
});

app.Run();





