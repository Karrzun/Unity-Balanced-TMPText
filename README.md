# README

A lightweight Unity component for balancing line breaks in TextMeshPro text blocks.

## Overview

TextMeshPro's standard word wrapping is practical, but it can sometimes produce uneven line lengths, especially in UI labels, menu copy, dialogue snippets, button descriptions, or short text blocks. This component provides a small utility script for cases where a more balanced visual layout is preferred.

## Features

- Balances multiline TextMeshPro text based on the available UI width
- Measures rendered text width using `TMP_Text.GetPreferredValues`
- Automatically recalculates when the source text or RectTransform width changes
- Provides a public `SetText(string newText)` method for runtime updates
- Disables TextMeshPro's default wrapping and applies explicit line breaks
- Uses a recursive search to find the best line distribution for the required number of lines

## Technical Highlights

- **Text measurement through TextMeshPro**  
  The component uses TextMeshPro's preferred-value calculation instead of estimating text width manually.

- **Width-aware layout**  
  The script reads the current width from the attached text component's `RectTransform`, making the result responsive to layout changes.

- **Balanced line scoring**  
  Candidate line arrangements are evaluated by comparing each line width to the average line width and minimizing the squared differences.

- **Runtime-friendly API**  
  Text can be updated through `SetText`, which marks the text for recalculation and immediately applies the balanced layout.

## Screenshots

<img width="1421" height="329" alt="Screenshot_Balanced_TMP" src="https://github.com/user-attachments/assets/972c5c3f-1328-4806-bfb1-453756f2bca5" />

##License

This project is licensed under the PolyForm Noncommercial License 1.0.0.
