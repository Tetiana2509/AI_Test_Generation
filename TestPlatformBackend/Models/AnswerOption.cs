using System;


namespace TestPlatformBackend.Models
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public double Weight { get; set; }  // �� 0 �� 1 ��� ���� ��������

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }

}
