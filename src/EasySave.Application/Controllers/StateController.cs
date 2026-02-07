using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;

namespace EasySave.Application.Controllers
{
    public class StateController
    {
        private readonly IFileStateService _stateService;

        public StateController(IFileStateService stateService)
        {
            _stateService = stateService;
        }

        public List<BackupProgress> GetCurrentStates()
        {
            return _stateService.ReadState();
        }
    }
}
