using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "AllowedCharsValidator", menuName = "TextMeshPro/Input Validators/Allowed Characters")]
public class AllowedCharacters : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch) {
        // Allow letters, numbers, and @ _ - .
        if (char.IsLetterOrDigit(ch) || ch == '@' || ch == '_' || ch == '-' || ch == '.') {
            text = text.Insert(pos, ch.ToString());
            pos++;
            return ch;
        }

        return '\0';
    }
}
