using UnityEngine;
using UnityEngine.UI;

public class PercentageDisplay : MonoBehaviour
{
    public Image hundredsImage;
    public Image tensImage;
    public Image unitsImage;
    public Image percentImage;

    public Sprite[] digitSprites;

    // Syst %
    public void UpdatePercentage(int percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 999);

        int hundreds = percentage / 100;
        int tens = (percentage % 100) / 10;
        int units = percentage % 10;

        if (digitSprites.Length < 10)
        {
            Debug.LogWarning("Il manque des sprites de chiffres !");
            return;
        }

        if (hundredsImage != null)
        {
            hundredsImage.sprite = digitSprites[hundreds];
            hundredsImage.enabled = percentage >= 100;
        }

        if (tensImage != null)
        {
            tensImage.sprite = digitSprites[tens];
            tensImage.enabled = percentage >= 10;
        }

        if (unitsImage != null)
        {
            unitsImage.sprite = digitSprites[units];
        }

        if (percentImage != null)
            percentImage.enabled = true;
    }

}
