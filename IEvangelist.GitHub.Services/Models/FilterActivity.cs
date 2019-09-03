using IEvangelist.GitHub.Services.Enums;
using System;

namespace IEvangelist.GitHub.Services.Models
{
    public class FilterActivity
    {
        public string Id { get; set; }

        public string MutationOrNodeId { get; set; }
        
        public ActivityType Type { get; set; }

        public DateTime WorkedOn { get; set; }

        public bool WasProfane { get; set; }
    }
}