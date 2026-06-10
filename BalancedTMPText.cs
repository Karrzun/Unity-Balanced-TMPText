using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BalancedTMPText : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;

    [TextArea]
    [SerializeField] private string sourceText;

    private string lastText;
    private float lastWidth;

    private void Awake()
    {
        if (tmpText == null)
            tmpText = GetComponent<TMP_Text>();

        tmpText.textWrappingMode = TextWrappingModes.NoWrap;
    }

    private void Update()
    {
        float currentWidth = GetAvailableWidth();

        if (sourceText != lastText || !Mathf.Approximately(currentWidth, lastWidth))
        {
            lastText = sourceText;
            lastWidth = currentWidth;

            ApplyBalancedText(sourceText);
        }
    }

    public void SetText(string newText)
    {
        sourceText = newText;
        lastText = null;

        ApplyBalancedText(sourceText);
    }

    private void ApplyBalancedText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            tmpText.text = "";
            return;
        }

        tmpText.text = BalanceLines(text);
    }

    private string BalanceLines(string text)
    {
        string[] words = text.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
            return text;

        float availableWidth = GetAvailableWidth();

        if (availableWidth <= 0f)
            return text;

        int requiredLineCount = GetMinimumRequiredLineCount(words, availableWidth);

        return BuildBestBalancedTextForExactLineCount(words, requiredLineCount, availableWidth);
    }

    private int GetMinimumRequiredLineCount(string[] words, float availableWidth)
    {
        int lineCount = 1;
        string currentLine = "";

        foreach (string word in words)
        {
            if (string.IsNullOrEmpty(currentLine))
            {
                currentLine = word;
                continue;
            }

            string candidate = currentLine + " " + word;

            if (MeasureTextWidth(candidate) <= availableWidth)
            {
                currentLine = candidate;
            }
            else
            {
                lineCount++;
                currentLine = word;
            }
        }

        return lineCount;
    }

    private string BuildBestBalancedTextForExactLineCount(string[] words, int targetLineCount, float availableWidth)
    {
        List<string> bestLines = null;
        float bestScore = float.MaxValue;

        SearchLineBreaks(
            words,
            availableWidth,
            targetLineCount,
            0,
            new List<string>(),
            ref bestLines,
            ref bestScore
        );

        if (bestLines == null)
            return string.Join(" ", words);

        return string.Join("\n", bestLines);
    }

    private void SearchLineBreaks(
        string[] words,
        float availableWidth,
        int targetLineCount,
        int startIndex,
        List<string> currentLines,
        ref List<string> bestLines,
        ref float bestScore)
    {
        int linesRemaining = targetLineCount - currentLines.Count;
        int wordsRemaining = words.Length - startIndex;

        if (linesRemaining <= 0)
            return;

        if (wordsRemaining < linesRemaining)
            return;

        if (linesRemaining == 1)
        {
            string finalLine = JoinWords(words, startIndex, words.Length);

            if (MeasureTextWidth(finalLine) > availableWidth)
                return;

            currentLines.Add(finalLine);

            float score = EvaluateLineBalance(currentLines);

            if (score < bestScore)
            {
                bestScore = score;
                bestLines = new List<string>(currentLines);
            }

            currentLines.RemoveAt(currentLines.Count - 1);
            return;
        }

        for (int endIndex = startIndex + 1; endIndex <= words.Length; endIndex++)
        {
            int wordsLeftAfterThisLine = words.Length - endIndex;

            if (wordsLeftAfterThisLine < linesRemaining - 1)
                break;

            string line = JoinWords(words, startIndex, endIndex);

            if (MeasureTextWidth(line) > availableWidth)
                break;

            currentLines.Add(line);

            SearchLineBreaks(
                words,
                availableWidth,
                targetLineCount,
                endIndex,
                currentLines,
                ref bestLines,
                ref bestScore
            );

            currentLines.RemoveAt(currentLines.Count - 1);
        }
    }

    private float EvaluateLineBalance(List<string> lines)
    {
        if (lines.Count <= 1)
            return 0f;

        float averageWidth = 0f;
        float[] widths = new float[lines.Count];

        for (int i = 0; i < lines.Count; i++)
        {
            widths[i] = MeasureTextWidth(lines[i]);
            averageWidth += widths[i];
        }

        averageWidth /= lines.Count;

        float score = 0f;

        foreach (float width in widths)
        {
            float difference = width - averageWidth;
            score += difference * difference;
        }

        return score;
    }

    private float MeasureTextWidth(string text)
    {
        Vector2 preferredValues = tmpText.GetPreferredValues(text);
        return preferredValues.x;
    }

    private float GetAvailableWidth()
    {
        RectTransform rectTransform = tmpText.rectTransform;
        return rectTransform.rect.width;
    }

    private string JoinWords(string[] words, int startIndex, int endIndex)
    {
        return string.Join(" ", words, startIndex, endIndex - startIndex);
    }
}