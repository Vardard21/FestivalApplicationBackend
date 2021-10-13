﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FestivalApplication.Data;
using FestivalApplication.Model;
using FestivalApplication.Model.DataTransferObjects;

namespace FestivalApplication.Controllers
{
    [Route("api/UserActivity")]
    [ApiController]
    public class UserActivitiesController : ControllerBase
    {
        private readonly DBContext _context;

        public UserActivitiesController(DBContext context)
        {
            _context = context;
        }

        // PUT: api/UserActivities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public Response<string> PutUserActivity(UserActivityDto userActivity)
        {
            //Create the response to be send out
            Response<string> response = new Response<string>();

            try {
                AuthenticateKey auth = new AuthenticateKey();
                if (auth.Authenticate(_context, Request.Headers["Authorization"]))
                {
                    //If the stageID is not 0, validate that the StageID exists and the stage is active
                    if (userActivity.StageID != 0)
                    {
                        if (_context.Stage.Where(x => x.StageID == userActivity.StageID && x.StageActive == true).ToList().Count() != 1)
                        {
                            response.InvalidOperation();
                            return response;
                        }
                    }
                    //Validate that the UserID exists
                    if (_context.User.Where(x => x.UserID == userActivity.UserID).ToList().Count() != 1)
                    {
                        response.InvalidData();
                        return response;
                    }

                    //Process the useractivity
                    //Check if the user is already in an activity or whether a new activity must be made
                    if (_context.UserActivity.Any(x => x.User.UserID == userActivity.UserID && x.Exit == default))
                    {
                        //Update the current UserActivity to exit a stage
                        if (userActivity.StageID == 0)
                        {
                            var ActiveActivity = _context.UserActivity.Where(x => x.User.UserID == userActivity.UserID && x.Exit == default).First();
                            ActiveActivity.Exit = DateTime.UtcNow;
                            _context.Entry(ActiveActivity).State = EntityState.Modified;
                        }
                        else
                        {
                            response.InvalidOperation();
                            return response;
                        }
                    }
                    else
                    {
                        //Create a new UserActivity for this UserID and StageID
                        UserActivity activity = new UserActivity();
                        activity.User = _context.User.Find(userActivity.UserID);
                        activity.Stage = _context.Stage.Find(userActivity.StageID);
                        activity.Entry = DateTime.UtcNow;
                        _context.UserActivity.Add(activity);
                    }

                    //Save the changes
                    if (_context.SaveChanges() > 0)
                    {
                        //Message was saved correctly
                        response.Success = true;
                        return response;
                    }
                    else
                    {
                        //Message was not saved correctly
                        response.ServerError();
                        return response;
                    }
                }
                else
                {
                    response.AuthorizationError();
                    return response;
                }
            }
            catch
            {
                response.ServerError();
                return response;
            }
        }
    }
}
