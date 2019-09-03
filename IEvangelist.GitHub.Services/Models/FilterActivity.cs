using IEvangelist.GitHub.Repository;
using IEvangelist.GitHub.Services.Enums;
using Newtonsoft.Json;
using System;

namespace IEvangelist.GitHub.Services.Models
{
    public class FilterActivity : BaseDocument
    {
        public string MutationOrNodeId { get; set; }
        
        public ActivityType Type { get; set; }

        public DateTime WorkedOn { get; set; }

        public bool WasProfane { get; set; }
    }
}