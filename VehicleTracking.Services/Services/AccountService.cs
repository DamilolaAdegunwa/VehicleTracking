﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VehicleTracking.Core.Entities;
using VehicleTracking.Core.Entities.EnumeratedTypes;
using VehicleTracking.Core.Model;
using VehicleTracking.Data.Context;
using System.IO;
using VehicleTracking.Core.Util;
using log4net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace VehicleTracking.Services.Services
{
    public interface IAccountService
    {
        Task<bool> AddUser(LoginModel model, string username);
        Task SignUp(LoginModel model);
    }
    public class AccountService : IAccountService
    {
        //private readonly ILogger<AccountService> log;
        //private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IServiceHelper _svcHelper;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);
        public AccountService(
            //ILogger<AccountService> logger
          //  , SignInManager<User> signInManager
             UserManager<User> userManager
            , RoleManager<Role> roleManager
            , ApplicationDbContext applicationDbContext
            , IHostingEnvironment hostingEnvironment
            , IServiceHelper serviceHelper

            )
        {
            //log = logger;
         //   _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
            _hostingEnvironment = hostingEnvironment;
            _svcHelper = serviceHelper;
        }
        public async Task<bool> AddUser(LoginModel model, string username)
        {
            try
            {
                #region validate credential

                //check that the model carries data
                if (model == null)
                {
                    throw new Exception("no input provided!");
                }
                //check for non-empty username 
                if (string.IsNullOrWhiteSpace(username))
                {
                    throw new Exception("Please login and retry");
                }

                //check that the user exist
                var user =  _applicationDbContext.Users.FirstOrDefault(x => x.UserName == username);
                if (user == null)
                {
                    throw new Exception("User does not exist");
                }
                //check that the person is an administrator
                if (user.UserType != UserType.Administrator)
                {
                    throw new Exception("you are not authorized to add user");
                }

                //check for valid usertype, validate the adtype if premium whether user can put premium ad
                #endregion
                user = new User
                {
                    UserName = model.UserName,
                    Email = model.UserName,
                    AccountConfirmationCode = CommonHelper.GenerateRandonAlphaNumeric(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    UserType = model.UserType,
                };
                var password = CommonHelper.GenerateRandonAlphaNumeric(); //"password";
                var creationStatus = await _userManager.CreateAsync(user, password);

                if (!creationStatus.Succeeded)
                {
                    throw new Exception(creationStatus.Errors?.FirstOrDefault().Description);
                }

                //you'd need to email the account credentials to the new user...
                try
                {
                    //first get the file
                    var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, Constants.Url.WelcomeEmail);
                    if (File.Exists(filePath))
                    {
                        var fileString = File.ReadAllText(filePath);
                        if (!string.IsNullOrWhiteSpace(fileString))
                        {
                            var admin = "";
                            if (model.UserType == UserType.Administrator) { admin = "(Administrator)"; }
                            fileString = fileString.Replace("{{UserName}}", $"{model.UserName} {admin}");
                            fileString = fileString.Replace("{{DefaultPassword}}", $"{password}");

                            _svcHelper.SendEMail(model.UserName, fileString, "Seven Peaks Software Co., Ltd: You are welcome on board!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"{ex.Message} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
                //return false;
                throw ex;
            }
        }

        public async Task SignUp(LoginModel model)
        {
            # region validate credential

            //check that the model carries data
            if (model == null)
            {
                throw new Exception("Invalid parameter");
            }
            //check that the model carries a password 
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new Exception("Please input a password");
            }

            //check that the user does not already exist
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);
            if (user != null)
            {
                throw new Exception("User already exist");
            }

            //check that the username is a valid email ( the password would be validate by the Identity builder)
            if (!Regex.IsMatch(model.UserName, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                throw new Exception("The UserName isn't Invalid Email");
            }

            //check for validate usertype
            #endregion

            #region sign up a new user
            try
            {
                user = new User
                {
                    UserName = model.UserName,
                    Email = model.UserName,
                    AccountConfirmationCode = CommonHelper.GenerateRandonAlphaNumeric(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    UserType = model.UserType,
                };
                var password = model.Password;// "password";
                var creationStatus = await _userManager.CreateAsync(user, password);

                if (creationStatus.Succeeded)
                {
                    try
                    {
                        //first get the file
                        var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, Constants.Url.WelcomeEmail);
                        if (File.Exists(filePath))
                        {
                            var fileString = File.ReadAllText(filePath);
                            if (!string.IsNullOrWhiteSpace(fileString))
                            {
                                var admin = "";
                                if(model.UserType == UserType.Administrator) { admin = "(Administrator)"; }
                                fileString = fileString.Replace("{{UserName}}", $"{model.UserName} {admin}");
                                fileString = fileString.Replace("{{DefaultPassword}}", $"{password}");

                                _svcHelper.SendEMail(model.UserName, fileString, "Seven Peaks Software Co., Ltd: You are welcome on board!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error($"{ex.Message} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace}");
                    }
                }
                else
                {
                    throw new Exception(creationStatus.Errors.FirstOrDefault()?.Description);
                }
            }
            catch (Exception ex)
            {
                var errMsg = $"an error occured while trying to signup. Please try again!";
                log.Error($"{errMsg} :: stack trace - {ex.StackTrace} :: exception message - {ex.Message}", ex);
                throw new Exception(errMsg);
            }
            #endregion
        }
    }
}
