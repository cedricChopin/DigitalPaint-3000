using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPickerExampleScript : MonoBehaviour
{
    CanvasManagement canvas;

    private void Start()
    {
        canvas = transform.GetComponent<CanvasManagement>();
    }
    public void ChooseColorButtonClick()
    {
        ColorPicker.Create(Color.red, "Choose the cube's color!", SetColor, ColorFinished, true);
    }
    private void SetColor(Color currentColor)
    {
        canvas.actualColor = currentColor;
    }

    private void ColorFinished(Color finishedColor)
    {
        Debug.Log("You chose the color " + ColorUtility.ToHtmlStringRGBA(finishedColor));
    }
}
