using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[Serializable]
public class ARGameHUD
{
    [Serializable]
    public class EditButton
    {
        public Image BG;
        public Text text;
        public Image icon;
    }

    [SerializeField] private Button infoButton = default, infoExitButton = default;

    [SerializeField] private Button editPosition = default, editRotation = default,
        editScale = default, viewMode = default, homeScreen = default, removeButton = default;

    [SerializeField] private List<EditButton> editButtons = new List<EditButton>();

    [SerializeField] private Button colorInfoButton = default, colorInfoExitButton = default;

    [SerializeField] private Toggle toggle = default;

    [SerializeField] private GameObject moveAround = default, instructionPanel = default,
        subEditModePanel = default, subScanPanel = default, subHint = default;
    [SerializeField] private Text instructionText = default;

    [SerializeField] private Animation instructionAnim = default, HUDAnim = default, colorInfoAnim = default;

    [SerializeField] private bool isKapyong;

    [SerializeField] private Color enableColor = new Color (34, 34, 34, 255);
    [SerializeField] private Color disableColor = new Color (200, 200, 200, 255);

    private ARStructureReferences structureReferences;
    private Action RemoveObject;
    private Action<bool> EnableColorCoding;

    private string instructionMessage;

    private const string HomeScreen = "Home";
    private const string KapyongHomeScreen = "Kapyong_Home";
    private const string popupAnimation = "NotificationPopupIn&Out";
    private const string colorInfoPopupIn = "ColorInfoPopupIn";
    private const string colorInfoPopupOut = "ColorInfoPopupOut";
    private const string HUDAnimation = "EditModeIn";
    private const string HUDStartup = "HUDStartup";
    private const string editPosMessage = "MOVE THE STRUCTURE\nTap and hold, then drag your finger to move the structure.";
    private const string editRotMessage = "ROTATE THE STRUCTURE\nTap and hold, then drag your finger to Rotate the structure.";
    private const string editScaleMessage = "RESIZE THE STRUCTURE\nPinch the screen to change the size of the structure.";

    private readonly List<Image> editButtonImages = new List<Image>();

    public void Initialize(ARStructureReferences structureReferences, Action RemoveObject, Action<bool> EnableColorCoding)
    {
        this.structureReferences = structureReferences;
        this.EnableColorCoding = EnableColorCoding;
        this.RemoveObject = RemoveObject;

        AssignRuntimeEvents();

        infoExitButton.onClick.AddListener(() => OnOpenInstructions(false));
        infoButton.onClick.AddListener(() => OnOpenInstructions(true));
        homeScreen.onClick.AddListener(LoadHomeScreen);

        if (colorInfoButton != null) colorInfoButton.onClick.AddListener(() => colorInfoAnim.Play(colorInfoPopupIn));
        if (colorInfoExitButton != null) colorInfoExitButton.onClick.AddListener(() => colorInfoAnim.Play(colorInfoPopupOut));

        if (removeButton != null) removeButton.onClick.AddListener(OnRemoveButtonPressed);
        if (toggle != null) toggle.onValueChanged.AddListener(OnEnableColorCoding);

        structureReferences.editMode = AREditMode.EditPosition;

        viewMode.gameObject.transform.parent.gameObject.SetActive(!structureReferences.isTerrain);
    }

    public void ShowTapHint(bool show) { }
    public void MoveCloser() { }
    public void MoveAround() { }

    public void ShowRemoveButton(bool show)
    {
        if (removeButton != null)
        {
            removeButton.gameObject.SetActive(show);
            if (!show) HUDAnim.Play(HUDStartup);
        }

        if (toggle != null) toggle.gameObject.SetActive(show);
    }

    public void OnSessionPaused(bool paused) => moveAround.TrySetActive(paused);

    public void LoadHomeScreen()
    {
        string scene = isKapyong ? KapyongHomeScreen : HomeScreen;
        SceneManager.LoadScene(scene);
    }

    public void OnOpenInstructions(bool open)
    {
        subHint.TrySetActive(false);
        instructionPanel.TrySetActive(open);
    }

    public void OnStructureCreated()
    {
        subEditModePanel.SetActive(true);
        subScanPanel.SetActive(false);
        subHint.SetActive(true);
        HUDAnim.Play(HUDAnimation);
        ShowRemoveButton(true);
    }

    public void OnForceRemoteCallback(AREditMode editMode)
    {
        structureReferences.editMode = editMode;

        switch (structureReferences.editMode)
        {
            case AREditMode.EditPosition: editPosition.onClick.Invoke(); break;
            case AREditMode.EditRotation: editRotation.onClick.Invoke(); break;
            case AREditMode.EditScale: editScale.onClick.Invoke(); break;
        }
    }

    private void OnRemoveButtonPressed()
    {
        RemoveObject?.Invoke();
        EnableColorCoding?.Invoke(false);
        toggle.isOn = false;
        ShowRemoveButton(false);
    }

    private void OnEnableColorCoding(bool isOn) => EnableColorCoding?.Invoke(isOn);

    private void ShowInstructionMessage()
    {
        if (structureReferences.editMode == AREditMode.View) return;

        switch (structureReferences.editMode)
        {
            case AREditMode.EditPosition: instructionMessage = editPosMessage; break;
            case AREditMode.EditRotation: instructionMessage = editRotMessage; break;
            case AREditMode.EditScale: instructionMessage = editScaleMessage; break;
        }

        if (instructionAnim.isPlaying) instructionAnim.Stop();

        instructionText.TrySetText(instructionMessage);
        instructionAnim.Play(popupAnimation);
    }

    private void AssignRuntimeEvents()
    {
        if (isKapyong)
        {
            editPosition.onClick.AddListener(() => OnSwitchEditModeKapyong(AREditMode.EditPosition, editButtons[0]));
            editScale.onClick.AddListener(() => OnSwitchEditModeKapyong(AREditMode.EditScale, editButtons[1]));
            editRotation.onClick.AddListener(() => OnSwitchEditModeKapyong(AREditMode.EditRotation, editButtons[2]));

            OnSwitchEditModeKapyong(AREditMode.EditPosition, editButtons[0]);

            return;
        }

        Image editPositionImage = editPosition.gameObject.GetComponent<Image>();
        Image editRotationImage = editRotation.gameObject.GetComponent<Image>();
        Image editScaleImage = editScale.gameObject.GetComponent<Image>();
        Image viewModeImage = viewMode.gameObject.GetComponent<Image>();

        editButtonImages.Add(editPositionImage);
        editButtonImages.Add(editRotationImage);
        editButtonImages.Add(editScaleImage);
        editButtonImages.Add(viewModeImage);

        editPosition.onClick.AddListener(() => OnSwitchEditMode(AREditMode.EditPosition, editPositionImage));
        editRotation.onClick.AddListener(() => OnSwitchEditMode(AREditMode.EditRotation, editRotationImage));
        editScale.onClick.AddListener(() => OnSwitchEditMode(AREditMode.EditScale, editScaleImage));
        viewMode.onClick.AddListener(() => OnSwitchEditMode(AREditMode.View, viewModeImage));
    }

    private void OnSwitchEditMode(AREditMode mode, Image image)
    {
        structureReferences.editMode = mode;
        ShowInstructionMessage();

        for (int i = 0; i < editButtonImages.Count; i++)
        {
            if (isKapyong) editButtonImages[i].color = image == editButtonImages[i] ? enableColor : disableColor;
            else editButtonImages[i].fillCenter = image == editButtonImages[i] ? true : false;
        }
    }

    private void OnSwitchEditModeKapyong(AREditMode mode, EditButton editButton)
    {
        structureReferences.editMode = mode;
        ShowInstructionMessage();

        for (int i = 0; i < editButtons.Count; i++)
        {
            editButtons[i].BG.color = editButtons[i] == editButton ? enableColor : disableColor;
            editButtons[i].icon.color = editButtons[i] == editButton ? disableColor : enableColor;
            editButtons[i].text.color = editButtons[i] == editButton ? disableColor : enableColor;
        }
    }
}