using Microsoft.AspNetCore.Identity;
using Monitor.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
 using Count4U.Service.Shared;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Count4U.Service.Core.Server.Data
{
	public class ApplicationUser : IdentityUser
	{
		public string FistName { get; set; }
		public string LastName { get; set; }
		public string DataServerAddress { get; set; }
		public string DataServerPort { get; set; }
		public string AccessKey { get; set; }
		public string CustomerCode { get; set; }
		public string BranchCode { get; set; }
		public string InventorCode { get; set; }
		public string DBPath { get; set; }

	}

	public static class ApplicationUserConvert
	{
		public static ProfileModel ToProfileModel(this ApplicationUser user)
		{
			ProfileModel profileModel = new ProfileModel();
			profileModel.ID = user.Id !=null ? user.Id : "";
			profileModel.DataServerAddress = user.DataServerAddress !=null ? user.DataServerAddress : "";
			//profileModel.DataServerPort = user.DataServerPort != null ? user.DataServerPort : "";
			profileModel.AccessKey = user.AccessKey != null ? user.AccessKey : "";
			profileModel.CustomerCode = user.CustomerCode != null ? user.CustomerCode : "";
			profileModel.BranchCode = user.BranchCode != null ? user.BranchCode : "";
			profileModel.InventorCode = user.InventorCode != null ? user.InventorCode : "";
			profileModel.DBPath = user.DBPath != null ? user.DBPath : "";
			return profileModel;
		}

		//public static ProfileModel ToProfileModel(this ClaimsPrincipal user)
		//{
		//	ProfileModel profileModel = new ProfileModel();
		//	//profileModel.ID =  user.Identity; ?? проблема надо рашить
		//	profileModel.DataServerAddress = user.Claims.FirstOrDefault(c => c.Type == ClaimEnum.DataServerAddress.ToString())?.Value;
		////	profileModel.DataServerPort = user.Claims.FirstOrDefault(c => c.Type == ClaimEnum.DataServerPort.ToString())?.Value;
		//	profileModel.AccessKey = user.Claims.FirstOrDefault(c => c.Type == ClaimEnum.AccessKey.ToString())?.Value;
		//	profileModel.CustomerCode = user.Claims.FirstOrDefault(c => c.Type == ClaimEnum.CustomerCode.ToString())?.Value;
		//	profileModel.BranchCode = user.Claims.FirstOrDefault(c => c.Type == ClaimEnum.BranchCode.ToString())?.Value;
		//	profileModel.InventorCode = user.Claims.FirstOrDefault(c => c.Type == ClaimEnum.InventorCode.ToString())?.Value;
		//	profileModel.DBPath = user.Claims.FirstOrDefault(c => c.Type == ClaimEnum.DBPath.ToString())?.Value;
		//	return profileModel;
		//}
	}
}
