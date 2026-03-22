using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class SoundManager : MonoBehaviour {
    // [Header("Звук накладывается то сколько ждать")]
    // [SerializeField] private float _soundDelay = 0.5f;
    //
    // [Header("Конфиги")]
    // [SerializeField] private List<SoundConfig> soundConfigs;
    // [Header("Тонкая настройка")]
    // [SerializeField] private int _poolSize;
    // [SerializeField] private float _fadeTime = 1f;
    // [SerializeField] private float _stepTiming = 0.2f;
    // [SerializeField] private GameObject _audioSourcesComponent;
    // [Header("Background Music")]
    // [SerializeField] private AudioSource _walkMusicSource;
    // [SerializeField] private AudioSource _flyMusicSource;
    // [SerializeField] private AudioSource _narratorSource;
    //
    // [Header("Mixer")]
    // [SerializeField] private AudioMixer _audioMixer;
    // [SerializeField] private AudioMixerGroup _musicMixerGroup;
    // [SerializeField] private AudioMixerGroup _soundMixerGroup;
    //
    // [Header("Mini-games objects")]
    // [SerializeField] private Trampoline[] _trampolines;
    // [SerializeField] private Basketball _basket;
    // [SerializeField] private AudioSource _basketballSource;
    //
    //
    // private Dictionary<SoundType, SoundConfig> _soundConfigDict = new ();
    // private List<AudioSource> _sources = new();
    // private bool _onAir;
    // private bool _allowToSound = true;
    //
    //
    //
    // [Inject] private PlayerStateManager _stateManager;
    // [Inject] private PlayerMovement _playerMovement;
    // [Inject] private PlayerBank _bank;
    // [Inject] private SettingsManager _settings;
    // [Inject] private PlayerSkinInventory _playerSkinInventory;
    // [Inject] private TrampolineManager _trampolineManager;
    // [Inject] private TasksManager _tasksManager;
    // [Inject] private PetOpenView _petOpenView;
    //
    //
    // private void Awake() {
    //     foreach (var _sound in soundConfigs) {
    //         _soundConfigDict[_sound.SoundType] = _sound;
    //     }
    //    
    //     // создаём пул
    //     for (int i = 0; i < _poolSize; i++) {
    //         CreateNewAudioSource();
    //     }
    //     PlayMusic(_soundConfigDict[SoundType.MainBackground]);
    // }
    //
    //
    // private void OnEnable() {
    //     // STATE CHANGES
    //     _stateManager.ChangeState += StateManagerOnChangeState;
    //     // PLAYER MOVE
    //     _playerMovement.JumpPressed += PlayerMovementOnJumpPressed;
    //     _playerMovement.DoubleJumpPressed += PlayerMovementOnJumpPressed;
    //     _playerMovement.RunningStateChanged += PlayerMovementOnRunningStateChanged;
    //     _playerMovement.Floored += PlayerMovementOnFloored;
    //     _playerMovement.SetBoost += PlayerMovementOnSetBoost;
    //     // BANK / WEAR
    //     _bank.BankNewMoneyPlus += OnMoneyPlus;
    //     _playerSkinInventory.SkinEquipped += SkinAction;
    //     // UI
    //     ButtonExtension.Click += OnUiButtonClick;
    //     // Settings
    //     _settings.MusicValueChanged += SettingsOnMusicValueChanged;
    //     _settings.EffectsValueChanged += SettingsOnEffectsValueChanged;
    //     // Minigames
    //     _basket.BallCollision += () => PlaySoundByType(SoundType.Ball);
    //     _trampolineManager.OnTrampolineJump += PlayTrampolineSound;
    //     // Task
    //     _tasksManager.TaskComplete += () => PlaySoundByType(SoundType.TaskComplete);
    //     _petOpenView.PetNewOpen += () => PlaySoundByType(SoundType.PetOpen);
    //     _petOpenView.PetCanasOpen += () => PlaySoundByType(SoundType.PetCanvasOpen);
    // }
    //
    //
    //
    // private AudioSource CreateNewAudioSource() {
    //     AudioSource source = _audioSourcesComponent.AddComponent<AudioSource>();
    //     _sources.Add(source);
    //     return source;
    // }
    //
    // private void PlaySoundByType(SoundType type) {
    //     if (!_soundConfigDict.TryGetValue(type, out var config)) {
    //         Debug.Log("Нет звука с типом " + type);
    //         return;
    //     }
    //     var clip = GetSource(config);
    //     
    //     AudioSource source = GetFreeSource(type);
    //     
    //     PlayInSource(source, clip, config);
    // }
    //
    // private void PlayTrampolineSound(Trampoline trampoline) {
    //     if (!_soundConfigDict.TryGetValue(SoundType.Trampoline, out var config)) {
    //         Debug.Log("Нет звука с типом " + SoundType.Trampoline);
    //         return;
    //     }
    //     AudioClip clip = GetSource(config);
    //     AudioSource source = trampoline.AudioSource;
    //     
    //     PlayInSource(source, clip, config);
    // }
    //
    // private void SkinAction(SkinItemConfig _) {
    //     BuyOrUnlock(0);
    // }
    //
    // private void Start() {
    //     SettingsOnMusicValueChanged(_settings.MusicValue);
    //     SettingsOnEffectsValueChanged(_settings.EffectsValue);
    // }
    //
    // private void SettingsOnMusicValueChanged(float value) {
    //     float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
    //     _audioMixer.SetFloat("MusicVolume", db);
    // }
    //
    // private void SettingsOnEffectsValueChanged(float value) {
    //     float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
    //     _audioMixer.SetFloat("EffectsVolume", db);
    // }
    //
    // private void OnUiButtonClick() {
    //     PlaySoundByType(SoundType.UIButton);
    // }
    //
    // public void UiButtonClick() {
    //     OnUiButtonClick();
    // }
    //
    // private void OnMoneyPlus(long _) {
    //     PlaySoundByType(SoundType.Money);
    // }
    //
    // private void BuyOrUnlock(long _) {
    //     if(!_allowToSound) return;
    //     Debug.Log("BuyOrUnlock sound");
    //     PlaySoundByType(SoundType.Unlock);
    //     StartCoroutine(WaitForSoundDelay(_soundDelay));
    // }
    //
    // private IEnumerator WaitForSoundDelay(float time) {
    //     _allowToSound = false;
    //     yield return new WaitForSeconds(time);
    //     _allowToSound = true;
    // }
    //
    // private void PlayerMovementOnSetBoost() {
    //     PlaySoundByType(SoundType.Boost);
    // }
    //
    //
    // private void PlayerMovementOnFloored() {
    //     // Можно звук приземления
    //     PlaySoundByType(SoundType.Step);
    //     PlayerMovementOnRunningStateChanged(_playerMovement.IsRunning);
    // }
    //
    // private CancellationTokenSource _cancellationTokenSource;
    // private void PlayerMovementOnRunningStateChanged(bool isRunning) {
    //     _cancellationTokenSource?.Cancel();
    //     if (isRunning) {
    //         _cancellationTokenSource = new CancellationTokenSource();
    //         StepCycleAsync(_cancellationTokenSource.Token).Forget();
    //     }
    // }
    //
    // private async UniTask StepCycleAsync(CancellationToken token) {
    //     while (!token.IsCancellationRequested) {
    //         if (!_playerMovement.IsGrounded || _onAir) {
    //             return;
    //         }
    //         PlaySoundByType(SoundType.Step);
    //         await UniTask.Delay(TimeSpan.FromSeconds(_stepTiming), cancellationToken: token);
    //     }
    // }
    //
    // private void PlayerMovementOnJumpPressed() {
    //     PlaySoundByType(SoundType.Jump);
    // }
    //
    //
    // private static void PlayInSource(AudioSource source, AudioClip clip, SoundConfig config) {
    //     source.clip = clip;
    //     source.volume = config.Volume;
    //     source.pitch = UnityEngine.Random.Range(config.PitchDiapasone.From, config.PitchDiapasone.To);
    //     source.loop = config.Loop;
    //     source.spatialBlend = config.SpatialBlend;
    //     source.outputAudioMixerGroup = config.MixerGroup;
    //     source.Play();
    // }
    //
    // private static AudioClip GetSource(SoundConfig config) {
    //     AudioClip clip = config.AudioClips.GetRandomElement();
    //     return clip;
    // }
    //
    //
    // public void PlayNarratorSound(AudioClip clip, float pitch) {
    //     AudioSource source = _narratorSource;
    //     
    //     // Обрезаем нарратора если наложилось
    //     if (source.isPlaying)
    //         source.Stop(); 
    //     
    //     
    //     source.clip = clip;
    //     source.volume = 1f;
    //     source.pitch = pitch;
    //     source.outputAudioMixerGroup = _soundMixerGroup;
    //     source.Play();
    // }
    //
    // private AudioSource GetFreeSource(SoundType type) {
    //
    //     if (type == SoundType.Ball) {
    //         return _basketballSource;
    //     }
    //     
    //     foreach (var source in _sources) {
    //         if (!source.isPlaying)
    //             return source;
    //     }
    //
    //     var newSource = CreateNewAudioSource();
    //     return newSource;
    // }
    //
    //
    // private void StateManagerOnChangeState(PlayerState state) {
    //     if (state == PlayerState.TrampolineJumping) return;
    //     
    //     if (state == PlayerState.Walking) {
    //         // Проигрывание MainBackground
    //         _onAir = false;
    //         PlayMusic(_soundConfigDict[SoundType.MainBackground]);
    //     }
    //     else if (state == PlayerState.Flight) {
    //         // Проигрывание FlyBackground
    //         _onAir = true;
    //         PlayMusic(_soundConfigDict[SoundType.FlyBackground]);
    //     }
    //     else if (state == PlayerState.Grounded || state == PlayerState.Cruisered) {
    //         PlaySoundByType(SoundType.Win);
    //     }
    //     
    // }
    //
    //
    // private void PlayMusic(SoundConfig config) 
    // {
    //     AudioSource targetSource = config.SoundType switch 
    //     {
    //         SoundType.MainBackground => _walkMusicSource,
    //         SoundType.FlyBackground => _flyMusicSource,
    //         _ => null
    //     };
    //     if (targetSource == null) return;
    //
    //     // Если этот источник уже играет нужный клип - выходим
    //     if (targetSource.isPlaying && targetSource.clip == config.AudioClips[0])
    //         return;
    //
    //     // Настраиваем источник
    //     targetSource.clip = config.AudioClips[0];
    //     targetSource.volume = 0f;
    //     targetSource.loop = config.Loop;
    //     targetSource.outputAudioMixerGroup = config.MixerGroup;
    //     targetSource.Play();
    //
    //     // Плавно затухаем все другие источники (кроме target)
    //     foreach (var src in new[] { _walkMusicSource, _flyMusicSource })
    //     {
    //         if (src == targetSource) continue;
    //
    //         if (src.isPlaying) 
    //         {
    //             src.DOFade(0f, _fadeTime).OnComplete(() =>
    //             {
    //                 src.Stop();
    //                 src.volume = 1f;
    //             });
    //         }
    //     }
    //
    //     // Плавное появление нового трека
    //     targetSource.DOFade(config.Volume, _fadeTime);
    // }
    //
    //
    //
    // private void OnDisable() {
    //     // STATE CHANGES
    //     _stateManager.ChangeState -= StateManagerOnChangeState;
    //     // PLAYER MOVE
    //     _playerMovement.JumpPressed -= PlayerMovementOnJumpPressed;
    //     _playerMovement.DoubleJumpPressed -= PlayerMovementOnJumpPressed;
    //     _playerMovement.RunningStateChanged -= PlayerMovementOnRunningStateChanged;
    //     _playerMovement.Floored -= PlayerMovementOnFloored;
    //     _playerMovement.SetBoost -= PlayerMovementOnSetBoost;
    //     // BANK
    //     _bank.BankNewMoneyPlus -= OnMoneyPlus;
    //     _bank.BankNewMoneyMinus -= BuyOrUnlock;
    //     // UI
    //     ButtonExtension.Click -= OnUiButtonClick;
    //     // Settings
    //     _settings.MusicValueChanged -= SettingsOnMusicValueChanged;
    //     _settings.EffectsValueChanged -= SettingsOnEffectsValueChanged;
    //     
    //     _cancellationTokenSource?.Cancel();
    //     _cancellationTokenSource?.Dispose();
    // }
    
}
