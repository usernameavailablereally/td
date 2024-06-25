using Custom;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class SlowMo : MonoBehaviour
{
    [SerializeField] private SlowMoConfig _config;
    [SerializeField] private KeyCode _triggerCode = KeyCode.N;
    [SerializeField] private KeyCode _stopTriggerCode = KeyCode.M;
    [SerializeField] private BackgroundMusic _bgMusic;
    [SerializeField] private MMProgressBar _ui;

    private float _slowMoAmount;
    private bool _isInSlowMo;

    void Start()
    {
        MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, _config.NormalTimeScale, 0, false, 1, true);
    }

    void Update()
    {
        UpdateState();
        UpdateSoundPitch();
    }

    private void UpdateState()
    {
        bool canStart = _slowMoAmount > _config.MinAmountToStart;
        bool canContinue = _slowMoAmount > 0;
        if (Input.GetKey(_triggerCode) && canStart)
        {
            StartSlowMo();
        }
        else if (Input.GetKey(_stopTriggerCode) || !canContinue)
        {
            StopSlowMo();
        }

        float delta = Time.unscaledDeltaTime;
        float change = _isInSlowMo ? -_config.SlowMoDrain : _config.SlowMoRestore;
        _slowMoAmount = Mathf.Clamp(_slowMoAmount + change * delta, 0, _config.SlowMoMeter);
        _ui.SetBar(_slowMoAmount, 0, _config.SlowMoMeter);
    }
    
    private void StartSlowMo()
    {
        _isInSlowMo = true;
        MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, _config.SlowMoTimeScale, 1, true, _config.EnterLerpSpeed, true);
    }

    private void StopSlowMo()
    {
        _isInSlowMo = false;
        MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, _config.NormalTimeScale, 1, true, _config.QuitLerpSpeed, true);
    }
    
    private void UpdateSoundPitch()
    {
        float timeScaleChange = Time.timeScale - _config.NormalTimeScale;
        float pitch = 1 + timeScaleChange;
        float bgMusicPitch = 1 + timeScaleChange * _config.BgMusicPitchFactor;

        MMSoundManager.Instance.SetPitch(pitch, new MMSoundManager.TracksPitch() {Music = bgMusicPitch});
    }
}
