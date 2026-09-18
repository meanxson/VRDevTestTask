using System;
using System.Collections.Generic;
using App.Application.Ports;
using App.Domain.Actions;
using App.Domain.Scenarios;
using UnityEngine;

namespace App.Infrastructure.Configuration
{
    /// <summary>
    /// Читает сценарий из JSON-файла проекта.
    ///
    /// Вся работа с внешним форматом заканчивается здесь: наружу отдаются уже
    /// проверенные доменные объекты. Любая проблема в файле превращается в
    /// исключение с текстом, по которому видно конкретное место ошибки —
    /// прикладной слой покажет его пользователю.
    /// </summary>
    public sealed class JsonScenarioSource : IScenarioSource
    {
        private readonly TextAsset _configuration;

        public JsonScenarioSource(TextAsset configuration)
        {
            // Незаполненную ссылку не считаем ошибкой программиста: это забытое
            // поле в инспекторе, и пользователь должен увидеть внятное сообщение,
            // а не исключение при старте сцены.
            _configuration = configuration;
        }

        public Scenario Load()
        {
            if (_configuration == null)
                throw new FormatException("Не назначен файл сценария в композиционном корне.");

            var dto = Deserialize();

            if (dto.groups == null || dto.groups.Length == 0)
                throw new FormatException("В сценарии нет ни одной группы шагов.");

            var groups = new List<StepGroup>(dto.groups.Length);
            for (var i = 0; i < dto.groups.Length; i++)
                groups.Add(ReadGroup(dto.groups[i], i));

            var title = string.IsNullOrWhiteSpace(dto.title) ? "Тренировка" : dto.title;

            return new Scenario(title, groups);
        }

        private ScenarioDto Deserialize()
        {
            try
            {
                var dto = JsonUtility.FromJson<ScenarioDto>(_configuration.text);

                if (dto == null)
                    throw new FormatException("Файл сценария пуст.");

                return dto;
            }
            catch (Exception exception) when (!(exception is FormatException))
            {
                throw new FormatException(
                    "Не удалось разобрать файл сценария \"" + _configuration.name + "\": " + exception.Message);
            }
        }

        private static StepGroup ReadGroup(StepGroupDto dto, int groupIndex)
        {
            if (dto == null)
                throw new FormatException(Where(groupIndex) + ": группа не задана.");

            if (string.IsNullOrWhiteSpace(dto.title))
                throw new FormatException(Where(groupIndex) + ": не указано название группы.");

            if (dto.steps == null || dto.steps.Length == 0)
                throw new FormatException(Where(groupIndex) + ": в группе нет шагов.");

            var steps = new List<Step>(dto.steps.Length);
            for (var i = 0; i < dto.steps.Length; i++)
                steps.Add(ReadStep(dto.steps[i], groupIndex, i));

            var briefing = string.IsNullOrWhiteSpace(dto.briefing) ? null : dto.briefing;

            return new StepGroup(dto.title, steps, briefing);
        }

        private static Step ReadStep(StepDto dto, int groupIndex, int stepIndex)
        {
            var where = Where(groupIndex, stepIndex);

            if (dto == null)
                throw new FormatException(where + ": шаг не задан.");

            if (string.IsNullOrWhiteSpace(dto.description))
                throw new FormatException(where + ": не указано описание шага.");

            var expectations = ReadExpectedActions(dto, where);

            // Номер шага не обязателен: если автор его не проставил,
            // нумеруем по порядку внутри группы.
            var id = dto.id > 0 ? dto.id : stepIndex + 1;

            return new Step(id, dto.description, expectations);
        }

        private static IReadOnlyList<ExpectedAction> ReadExpectedActions(StepDto dto, string where)
        {
            // Развёрнутая запись имеет приоритет: она позволяет описать шаг,
            // который ждёт нескольких действий сразу.
            if (dto.expectedActions != null && dto.expectedActions.Length > 0)
            {
                var expectations = new List<ExpectedAction>(dto.expectedActions.Length);

                for (var i = 0; i < dto.expectedActions.Length; i++)
                {
                    var expectation = dto.expectedActions[i];

                    if (expectation == null)
                        throw new FormatException(where + ": пустое ожидаемое действие №" + (i + 1) + ".");

                    expectations.Add(ReadExpectedAction(expectation.action, expectation.target, where));
                }

                return expectations;
            }

            return new[] { ReadExpectedAction(dto.expectedAction, dto.target, where) };
        }

        private static ExpectedAction ReadExpectedAction(string action, string target, string where)
        {
            if (string.IsNullOrWhiteSpace(target))
                throw new FormatException(where + ": не указана цель действия.");

            try
            {
                return new ExpectedAction(ActionTypeParser.Parse(action), target);
            }
            catch (FormatException exception)
            {
                throw new FormatException(where + ": " + exception.Message);
            }
        }

        private static string Where(int groupIndex)
        {
            return "Группа №" + (groupIndex + 1);
        }

        private static string Where(int groupIndex, int stepIndex)
        {
            return Where(groupIndex) + ", шаг №" + (stepIndex + 1);
        }
    }
}
