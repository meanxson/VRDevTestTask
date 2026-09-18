using System.Text;
using App.Domain.Execution;

namespace App.Presentation.Ui
{
    /// <summary>
    /// Превращает состояние прохождения в текст для панелей интерфейса.
    ///
    /// Собран в одном месте, чтобы чек-лист в сцене и экран итогов показывали
    /// статусы одинаково. Цвет и значок дублируют друг друга намеренно: в VR
    /// текст читается хуже, чем на мониторе, и одного цвета мало.
    /// </summary>
    public static class StepStatusFormatter
    {
        /// <summary>Название статуса шага для пользователя.</summary>
        public static string Describe(StepStatus status)
        {
            switch (status)
            {
                case StepStatus.Active: return "выполняется";
                case StepStatus.Succeeded: return "выполнен";
                case StepStatus.Failed: return "выполнен с ошибкой";
                case StepStatus.Skipped: return "пропущен";
                default: return "ожидает";
            }
        }

        /// <summary>Название статуса группы для экрана итогов.</summary>
        public static string Describe(GroupStatus status)
        {
            switch (status)
            {
                case GroupStatus.Active: return "выполняется";
                case GroupStatus.Completed: return "завершена";
                case GroupStatus.Interrupted: return "прервана: нарушен порядок шагов";
                default: return "не начата";
            }
        }

        /// <summary>Строка чек-листа для одного шага.</summary>
        public static string FormatStep(StepProgress step)
        {
            return "<color=#" + ColorOf(step.Status) + ">" + MarkerOf(step.Status) + " " +
                   step.Definition.Description + " — " + Describe(step.Status) + "</color>";
        }

        /// <summary>Заголовок и шаги группы для экрана итогов.</summary>
        public static string FormatGroup(StepGroupProgress group)
        {
            var builder = new StringBuilder();

            builder.Append("<b>").Append(group.Definition.Title).Append("</b> — ")
                .Append(Describe(group.Status)).AppendLine();

            var steps = group.Steps;
            for (var i = 0; i < steps.Count; i++)
                builder.AppendLine(FormatStep(steps[i]));

            return builder.ToString();
        }

        private static string MarkerOf(StepStatus status)
        {
            switch (status)
            {
                case StepStatus.Active: return "➤";
                case StepStatus.Succeeded: return "✔";
                case StepStatus.Failed: return "✖";
                case StepStatus.Skipped: return "—";
                default: return "•";
            }
        }

        private static string ColorOf(StepStatus status)
        {
            switch (status)
            {
                case StepStatus.Active: return "FFC107";
                case StepStatus.Succeeded: return "7BC67B";
                case StepStatus.Failed: return "E57373";
                case StepStatus.Skipped: return "9E9E9E";
                default: return "FFFFFF";
            }
        }
    }
}
