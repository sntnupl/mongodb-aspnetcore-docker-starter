using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoCore.DbDriver;
using MongoCore.DbDriver.Documents;
using MongoCore.WebApi.Helpers;
using MongoCore.WebApi.Models.Users;
using MongoDB.Bson;
using NLog;

namespace MongoCore.WebApi.Controllers
{
    [Authorize]
    [Route("api/admin")]
    public class AdminController : Controller
    {
        private readonly IRepository _appRepository;
        private readonly IAppConfig _appConfig;
        private readonly ILogger<AuthController> _logger;

        public AdminController(ILogger<AuthController> logger, IAppConfig appConfig, IRepository appRepository)
        {
            _appRepository = appRepository;
            _appConfig = appConfig;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _appRepository.GetAllUsersAsync();
            var result = Mapper.Map<IEnumerable<AppUserDto>>(users);
            return Ok(result);
        }
        
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _appRepository.GetUserAsync(ObjectId.Parse(id));
            if (null == user) return NotFound();
            var result = Mapper.Map<AppUserDto>(user);
            return Ok(result);
        }
        
        [HttpGet("users/{idUser}/tasks")]
        public async Task<IActionResult> GetTasks(string idUser)
        {
            var tasks = await _appRepository.GetAllTasksAsync(ObjectId.Parse(idUser));
            if (null == tasks) return NotFound();
            var result = Mapper.Map<IEnumerable<AppUserTaskDto>>(tasks);
            return Ok(result);
        }
        
        [HttpGet("users/{idUser}/tasks/{idTask}", Name="GetTaskForUser")]
        public async Task<IActionResult> GetTask(string idUser, string idTask)
        {
            var task = await _appRepository.GetTaskAsync(ObjectId.Parse(idUser), ObjectId.Parse(idTask));
            if (null == task) return NotFound();
            var result = Mapper.Map<AppUserTaskDto>(task);
            return Ok(result);
        }

        [HttpPost("users/{id}/tasks")]
        public async Task<IActionResult> AddTaskForUser(string id, [FromBody] AddTaskDto addTaskDto)
        {
            if (null == addTaskDto) return BadRequest();
            
            var userId = ObjectId.Parse(id);
            if (!await _appRepository.UserExistsAsync(userId)) return NotFound();

            var taskDoc = Mapper.Map<TaskDocument>(addTaskDto);
            var newTaskCreated = await _appRepository.AddTaskAsync(userId, taskDoc);

            if (null == newTaskCreated) return StatusCode(500, "Failed to add Task for User");
            var taskDto = Mapper.Map<AppUserTaskDto>(newTaskCreated);
            return CreatedAtRoute("GetTaskForUser", 
                                    new {idUser = id, idTask = taskDto.Id},
                                    taskDto);
        }

        [HttpDelete("users/{idUser}/tasks/{idTask}")]
        public async Task<IActionResult> DeleteTaskForUser(string idUser, string idTask)
        {
            var userId = ObjectId.Parse(idUser);
            if (!await _appRepository.UserExistsAsync(userId)) return NotFound();

            var taskId = ObjectId.Parse(idTask);
            var task = await _appRepository.GetTaskAsync(userId, taskId);
            if (null == task) return NotFound();

            var result = await _appRepository.DeleteTaskAsync(userId, taskId);
            if (!result) return StatusCode(500, "Failed to Delete Book, please try again later.");
            return NoContent();
        }

        [HttpPut("users/{idUser}/tasks/{idTask}")]
        public async Task<IActionResult> UpdateTaskForUser(string idUser, string idTask, [FromBody] TaskUpdateDto taskUpdateDto)
        {
            if (null == taskUpdateDto) return BadRequest();
            
            if (taskUpdateDto.Name == taskUpdateDto.Description) 
                ModelState.AddModelError(nameof(TaskUpdateDto), "Task Name and Description should not match");
            if (!ModelState.IsValid) return new UnprocessableEntityObjectResult(ModelState);

            var userId = ObjectId.Parse(idUser);
            if (!await _appRepository.UserExistsAsync(userId)) return NotFound();

            var taskId = ObjectId.Parse(idTask);
            var taskToUpdate = await _appRepository.GetTaskAsync(userId, taskId);
            if (null == taskToUpdate) return NotFound(); // won't allow Upserting

            Mapper.Map(taskUpdateDto, taskToUpdate);
            var result = await _appRepository.UpdateTaskAsync(userId, taskId, taskToUpdate);
            
            if (!result) throw new Exception($"Failed to update task: {idTask}, for author: {idUser}");
            return NoContent();
        }
        
        // todo:
        // 2. allow task add/delete/edit accessible to ONLY Admins
        
    }
}