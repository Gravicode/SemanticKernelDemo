﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel.Diagnostics;

namespace CustomSemanticKernel;

public static class Verify
{
    private static readonly Regex s_asciiLettersDigitsUnderscoresRegex = new("^[0-9A-Za-z_]*$");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void NotNull([NotNull] object? obj, string message)
    {
        if (obj is null)
        {
            ThrowValidationException("NullValue", message);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void NotEmpty([NotNull] string? str, string message)
    {
        NotNull(str, message);
        if (string.IsNullOrWhiteSpace(str))
        {
            ThrowValidationException("EmptyValue", message);
        }
    }

    internal static void ValidSkillName([NotNull] string? skillName)
    {
        NotEmpty(skillName, "The skill name cannot be empty");
        if (!s_asciiLettersDigitsUnderscoresRegex.IsMatch(skillName))
        {
            ThrowInvalidName("skill name", skillName);
        }
    }

    internal static void ValidFunctionName([NotNull] string? functionName)
    {
        NotEmpty(functionName, "The function name cannot be empty");
        if (!s_asciiLettersDigitsUnderscoresRegex.IsMatch(functionName))
        {
            ThrowInvalidName("function name", functionName);
        }
    }

    internal static void ValidFunctionParamName([NotNull] string? functionParamName)
    {
        NotEmpty(functionParamName, "The function parameter name cannot be empty");
        if (!s_asciiLettersDigitsUnderscoresRegex.IsMatch(functionParamName))
        {
            ThrowInvalidName("function parameter name", functionParamName);
        }
    }

    internal static void StartsWith(string text, string prefix, string message)
    {
        NotEmpty(text, "The text to verify cannot be empty");
        NotNull(prefix, "The prefix to verify is empty");
        if (!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            ThrowValidationException("MissingPrefix", message);
        }
    }

    internal static void DirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            ThrowValidationException("DirectoryNotFound", $"Directory not found: {path}");
        }
    }

    /// <summary>
    /// Make sure every function parameter name is unique
    /// </summary>
    /// <param name="parameters">List of parameters</param>
    internal static void ParametersUniqueness(IList<ParameterView> parameters)
    {
        int count = parameters.Count;
        if (count > 0)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < count; i++)
            {
                ParameterView p = parameters[i];

                NotEmpty(p.Name, "The parameter name is empty");

                if (!seen.Add(p.Name))
                {
                    throw new Exception(
                       
                        $"The function has two or more parameters with the same name '{p.Name}'");
                }
            }
        }
    }

    internal static void GreaterThan<T>(T value, T min, string message) where T : IComparable<T>
    {
        int cmp = value.CompareTo(min);

        if (cmp <= 0)
        {
            throw new Exception( message);
        }
    }

    public static void LessThan<T>(T value, T max, string message) where T : IComparable<T>
    {
        int cmp = value.CompareTo(max);

        if (cmp >= 0)
        {
            throw new Exception( message);
        }
    }

    [DoesNotReturn]
    private static void ThrowInvalidName(string kind, string name) =>
        throw new Exception(
            
            $"A {kind} can contain only ASCII letters, digits, and underscores: '{name}' is not a valid name.");

    [DoesNotReturn]
    private static void ThrowValidationException(string errorCodes, string message) =>
        throw new Exception($"{errorCodes}: {message}");
}


