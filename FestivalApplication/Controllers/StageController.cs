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



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FestivalApplication.Controllers
{
    [Route("api/Stage")]
    [ApiController]
    public class StageController : ControllerBase
    {
        private readonly DBContext _context;

        public StageController(DBContext context)
        {
            _context = context;
        }
        // GET: api/<StageController>
        [HttpGet]
        public Response<List<StagesRequestDto>> GetStages()
        {
            //creates a response variable to be sent
            Response<List<StagesRequestDto>> response = new Response<List<StagesRequestDto>>();
            try
            {
                AuthenticateKey auth = new AuthenticateKey();
                if (auth.Authenticate(_context, Request.Headers["Authorization"]))
                {
                    //create a stages variable to be checked
                    var stagesstatus = _context.Stage
                        .Where(x => x.StageActive == true)
                        .ToList();


                    //create a list of active stages
                    List<StagesRequestDto> ActiveStages = new List<StagesRequestDto>();


                        //create a for loop for each stage in stage status
                        foreach (Stage stage in stagesstatus)
                        {
                            //Create a new Stage Request DTO and fill the id and name
                            StagesRequestDto dto = new StagesRequestDto();
                            dto.StageID = stage.StageID;
                            dto.StageName = stage.StageName;
                            //Find the Current Song, temporarily only manually defined
                            dto.CurrentSong = "Thunderstruck by AC/DC";
                            ////Find the amount of active users in the stage
                            dto.NumberOfUsers = _context.UserActivity
                                .Where(x => x.Stage.StageID == stage.StageID)
                                .Where(x => x.Exit == default)
                                .Count();
                            //Add the new object to the return list
                            ActiveStages.Add(dto);
                        }

                        response.Success = true;
                        response.Data = ActiveStages;
                        return response;

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

        // GET: api/<StageController2>
        [HttpGet("{id}")]
        public Response<List<StageUsersDto>> GetStageUsers(int id)
        {
            //creates a response variable to be sent
            Response<List<StageUsersDto>> response = new Response<List<StageUsersDto>>();
            try
            {
                AuthenticateKey auth = new AuthenticateKey();
                if (auth.Authenticate(_context, Request.Headers["Authorization"]))
                {
                    //checks if stage exists
                    if (!_context.Stage.Any(x => x.StageID == id))
                    {
                        //Stage does not exist
                        response.InvalidData();
                        return response;
                    }

                    //create a stages variable to be checked
                    var stageusers = _context.UserActivity
                        .Where(x => x.Stage.StageID == id && x.Exit == default)
                        .ToList();


                    List<StageUsersDto> ActiveUsers = new List<StageUsersDto>();

                    if (!ActiveUsers.Any())
                    {

                        //create a for loop for each stage in stage status
                        foreach (UserActivity useractivity in stageusers)
                        {
                            //Create a new Stage Request DTO and fill the id and name
                            StageUsersDto dto = new StageUsersDto();
                            User user = _context.User.Find(useractivity.User.UserID);
                            dto.UserID = user.UserID;
                            dto.UserName = user.UserName;
                            dto.UserRole = user.Role;


                            //Add the new object to the return list
                            ActiveUsers.Add(dto);
                        }

                        response.Success = true;
                        response.Data = ActiveUsers;
                        return response;
                    }
                    else
                    {
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

        // POST: api/Stage
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Response<string> PostStage(StageCreateDto stagecreatedto)
        {
            //creates a response variable to be sent
            Response<string> response = new Response<string>();
            try
            {
                AuthenticateKey auth = new AuthenticateKey();
                if (auth.Authenticate(_context, Request.Headers["Authorization"]))
                {
                    //Validate that the requestor is an admin
                    if (!_context.Authentication.Any(x => x.User.Role == "admin" && x.AuthenticationKey == Request.Headers["Authorization"]))
                    {
                        //User changing the role is not an admin
                        response.InvalidOperation();
                        return response;
                    }

                    //create a new stage to be inserted in DB
                    Stage newStage = new Stage();

                    //check if the new stage has name of an existing stage
                    var stagefound = _context.Stage.Where(x => x.StageName == stagecreatedto.StageName).ToList();


                    if (stagefound.Count() == 1)
                    {

                        response.InvalidOperation();
                        response.Data = "Stage with the same name already exists";
                        return response;

                    }
                    else
                    {
                        newStage.StageName = stagecreatedto.StageName;
                        if (stagecreatedto.StageName.Any())
                        {
                            newStage.StageActive = stagecreatedto.StageActive;
                        }
                        else
                        {
                            newStage.StageActive = true;
                        }
                        _context.Stage.Add(newStage);

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
                            response.Data = "Error while saving stage";
                            return response;
                        }
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

        // PUT: api/StagesController/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public Response<string> PutUser(StageUpdateDto changestage)
        {
            //Create a new response
            Response<string> response = new Response<string>();
            try
            {
                AuthenticateKey auth = new AuthenticateKey();
                if (auth.Authenticate(_context, Request.Headers["Authorization"]))
                {
                    //Validate that the requestor is an admin
                    if (!_context.Authentication.Any(x => x.User.Role == "admin" && x.AuthenticationKey == Request.Headers["Authorization"]))
                    {
                        //User changing the role is not an admin
                        response.InvalidOperation();
                        return response;
                    }

                    //Check if the stageID exists
                    if ((_context.Stage.Any(x => x.StageID == changestage.StageID)))

                    {
                        //Check if the state is actually different
                        if (_context.Stage.Any(x => x.StageID == changestage.StageID && x.StageActive == changestage.StageActive))
                        {
                            //Stage is already at that state
                            response.InvalidData();
                            response.Data = "Stage is already in that state";
                            return response;
                        }
                        //Change the state
                        Stage stage = _context.Stage.Find(changestage.StageID);
                        stage.StageActive = changestage.StageActive;
                        _context.Entry(stage).State = EntityState.Modified;

                        //Save the changes
                        if (_context.SaveChanges() > 0)
                        {
                            //Stage was updated succesfully
                            response.Success = true;
                            return response;
                        }
                        else
                        {
                            //Error in updating stage
                            response.ServerError();
                            response.Data = "State has not been updated";
                            return response;
                        }
                    }
                    else
                    {
                        //Stage does not exist
                        response.Success = false;
                        response.InvalidData();
                        response.Data = "Stage does not exist";
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

        // DELETE api/<LoginController>/5
        [HttpDelete("{StageID}")]
        public Response<string> Delete(int StageID)
        {
            //Create a new response
            Response<string> response = new Response<string>();
            try
            {
                //Validate the authentication key
                AuthenticateKey auth = new AuthenticateKey();
                if (auth.Authenticate(_context, Request.Headers["Authorization"]))
                {
                    //Validate that the user deleting the stage is an admin
                    if(!_context.Authentication.Any(x=> x.AuthenticationKey == Request.Headers["Authorization"] && x.User.Role == "admin"))
                    {
                        response.InvalidOperation();
                        return response;
                    }
                    //Validate that the stage exists
                    Stage stage = _context.Stage.Find(StageID);
                    if(stage == null)
                    {
                        response.InvalidData();
                        return response;
                    }

                    //Validate that the stage is inactive
                    if (stage.StageActive)
                    {
                        response.InvalidOperation();
                        return response;
                    }

                    //Validate that the stage is empty
                    if (_context.UserActivity.Where(x=> x.Stage == stage && x.Exit == default).Count() > 0)
                    {
                        response.InvalidOperation();
                        return response;
                    }

                    //Delete the stage
                    stage.Archived = true;
                    _context.Entry(stage).State = EntityState.Modified;
                    if(_context.SaveChanges() > 0)
                    {
                        response.Success = true;
                        return response;
                    }
                    else
                    {
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

        [Route("api/Stage/all")]
        // GET: api/<StageController>
        [HttpGet]
        public Response<List<StagesFullRequestDto>> GetallStages()
        {
            //creates a response variable to be sent
            Response<List<StagesFullRequestDto>> response = new Response<List<StagesFullRequestDto>>();
            try
            {
                AuthenticateKey auth = new AuthenticateKey();
                if (auth.Authenticate(_context, Request.Headers["Authorization"]))
                {
                    //create a stages variable to be checked
                    var stagesstatus = _context.Stage
                        .Where(x => x.Archived == false)
                        .ToList();


                    //create a list of active stages
                    List<StagesFullRequestDto> ActiveStages = new List<StagesFullRequestDto>();

                    if (!ActiveStages.Any())
                    {

                        //create a for loop for each stage in stage status
                        foreach (Stage stage in stagesstatus)
                        {
                            //Create a new Stage Request DTO and fill the id and name
                            StagesFullRequestDto dto = new StagesFullRequestDto();
                            dto.StageID = stage.StageID;
                            dto.StageName = stage.StageName;
                            dto.StageActive = stage.StageActive;
                            //Find the Current Song, temporarily only manually defined
                            dto.CurrentSong = "Thunderstruck by AC/DC";
                            ////Find the amount of active users in the stage
                            dto.NumberOfUsers = _context.UserActivity
                                .Where(x => x.Stage.StageID == stage.StageID)
                                .Where(x => x.Exit == default)
                                .Count();
                            //Add the new object to the return list
                            ActiveStages.Add(dto);
                        }

                        response.Success = true;
                        response.Data = ActiveStages;
                        return response;
                    }
                    else
                    {
                        response.Success = false;
                        response.Data = ActiveStages;
                        response.ErrorMessage.Add(1);
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
                response.Success = false;
                response.ErrorMessage.Add(1);
                return response;
            }
        }
    }
}

