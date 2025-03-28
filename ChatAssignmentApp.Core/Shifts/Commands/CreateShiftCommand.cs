﻿using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Core.Shifts.Interfaces;
using ChatAssignmentApp.Core.Shifts.Models;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using ChatAssignmentApp.Queuing.Services;

namespace ChatAssignmentApp.Core.Shifts.Commands
{
    public class CreateShiftCommand : ICreateShiftCommand
    {
        private readonly IQueueService _queueService;
        private readonly IShiftStorageService _shiftStorageService;

        public CreateShiftCommand(
            IQueueService queueService,
            IShiftStorageService shiftStorageService)
        {
            _queueService = queueService;
            _shiftStorageService = shiftStorageService;
        }

        public async Task<CommandResult<ShiftModel>> ExecuteAsync(
            CreateShiftModel model)
        {
            if (model.TotalAgents <= 0)
                return new CommandResult<ShiftModel>(false, "Total number of agents cannot be 0. ");

            if (_shiftStorageService.DoesShiftExist())
                return new CommandResult<ShiftModel>(false, "A shift exists. End the shift first before creating a new one. ");

            var agents = new List<Agent>();

            if (model.NumberOfJuniorAgents > 0)
                agents.AddRange(CreateAgents(model.NumberOfJuniorAgents, AgentSeniorityType.Junior, false));

            if (model.NumberOfMidAgents > 0)
                agents.AddRange(CreateAgents(model.NumberOfMidAgents, AgentSeniorityType.Mid, false));

            if (model.NumberOfSeniorAgents > 0)
                agents.AddRange(CreateAgents(model.NumberOfSeniorAgents, AgentSeniorityType.Senior, false));

            if (model.NumberOfLeadAgents > 0)
                agents.AddRange(CreateAgents(model.NumberOfLeadAgents, AgentSeniorityType.Lead, false));

            var shift = new Shift(
                model.ShiftStart,
                agents);

            if (model.IsOverflowAgentsAvailable)
                shift.AddOverflowAgents(CreateAgents(6, AgentSeniorityType.Junior, true));

            _shiftStorageService.CreateShift(shift);

            await _queueService.CreateQueues(
                shift.MaxChatsToQueue,
                shift.IsOverflowAgentsAvailable);

            return new CommandResult<ShiftModel>(
                new ShiftModel(shift));
        }

        private List<Agent> CreateAgents(
            short numberOfAgents,
            AgentSeniorityType agentSeniorityType,
            bool isOverflowAgent)
        {
            var agents = new List<Agent>();

            for (short i = 0; i <= numberOfAgents - 1; i++)
            {
                agents.Add(
                    new Agent(
                        i + 1,
                        agentSeniorityType,
                        isOverflowAgent));
            }

            return agents;
        }
    }
}
