using System.Collections.Generic;
using System;

public class User
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string PhoneNumber { get; set; }
    public string Message { get; set; }

    public override string ToString()
    {
        return String.Format($"First Name: {FirstName}, Last Name: {LastName}, Phone Number: {PhoneNumber}");
    }

}