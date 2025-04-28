using System;
using System.Collections.Generic;

namespace TestPlatformBackend.DTOs
{
    public class SubmitTestDto
    {
        public int UserId { get; set; }
        public int FullTestId { get; set; }
        public DateTime SubmittedAt { get; set; }

        public List<AnswerSubmissionDto> AnswerSubmissions { get; set; } = new();
    }

    public class AnswerSubmissionDto
    {
        public int QuestionId { get; set; }
        public int AnswerOptionId { get; set; }
    }
}
