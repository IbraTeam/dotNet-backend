﻿using dotNetBackend.models.Enums;

namespace dotNetBackend.models.DTO
{
    public class CreatePair
    {
        public DateTime DateTime { get; set; }
        public int RepeatCount { get; set; }
        public PairNumber PairNumber { get; set; }
        public Guid TeacherId { get; set; }
        public Guid KeyId { get; set; }
        public string PairName { get; set; }
    }
}
