﻿using Microsoft.EntityFrameworkCore;
using RestApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Server.Data
{
	public class CustomerContext : DbContext
	{
		public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		public DbSet<Customer> Customers { get; set; }
	}

}


//Add-Migration InitialCreate -Context CustomerContext
//Update-Database	-Context CustomerContext