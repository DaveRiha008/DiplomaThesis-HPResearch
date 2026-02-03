using UnityEngine;
using DG.Tweening;
using TMPro;

public static class UIFlashingNumbers
{
	const float _flashDuration = .8f;
	const float _fadeInDuration = 0.0f;
	const float _fadeOutDuration = _flashDuration - _fadeInDuration;

	static Vector2 _moveOffset = new Vector2(.3f, .9f);

	/// <summary>
	/// Will show a flashing number label at the given parent transform's position plus startOffset, moving by moveOffset, fading in and out.
	/// </summary>
	public static void ShowFlashingNumber(Transform parentTransform, int number, Color color, Vector2 startOffset, Vector2 moveOffset, int fontSize = 5, float fadeInDuration = _fadeInDuration, float fadeOutDuration = _fadeOutDuration)
    {
		GameObject labelObject = new GameObject("FlashingNumberLabel");
		TextMeshPro flashingNumberLabel = labelObject.AddComponent<TextMeshPro>();

		flashingNumberLabel.text = number.ToString();
		flashingNumberLabel.fontSize = fontSize;
		flashingNumberLabel.alignment = TextAlignmentOptions.Center;

		flashingNumberLabel.transform.SetParent(parentTransform);
		flashingNumberLabel.transform.localPosition = startOffset;
		flashingNumberLabel.color = color.WithAlpha(0);

		flashingNumberLabel.transform.DOLocalMove(flashingNumberLabel.transform.localPosition + (Vector3)moveOffset, fadeInDuration+fadeOutDuration).SetEase(Ease.OutCubic)
			.SetLink(parentTransform.gameObject);

		flashingNumberLabel.DOColor(color.WithAlpha(1), fadeInDuration).SetLink(parentTransform.gameObject)
			.OnComplete(() =>
		{
			flashingNumberLabel.DOColor(color.WithAlpha(0), fadeOutDuration).SetLink(parentTransform.gameObject)
			.OnComplete(() =>
			{
				Object.Destroy(flashingNumberLabel.gameObject);
			});
		});
	}

	/// <summary>
	/// Will show a flashing number label at the given parent transform's position fading in and out.
	/// </summary>
	public static void ShowFlashingNumber(Transform parentTransform, int number, Color color)
	{
		ShowFlashingNumber(parentTransform, number, color, Vector2.zero, _moveOffset, 5, _fadeInDuration, _fadeOutDuration);
	}

	/// <summary>
	/// Will show a flashing number label at the given parent transform's position plus startOffset, fading in and out.
	/// </summary>
	public static void ShowFlashingNumber(Transform parentTransform, int number, Color color, Vector2 startOffset)
	{
		ShowFlashingNumber(parentTransform, number, color, startOffset, _moveOffset, 5,_fadeInDuration, _fadeOutDuration);
	}
}
