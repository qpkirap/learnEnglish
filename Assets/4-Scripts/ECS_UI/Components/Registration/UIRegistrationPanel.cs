using System;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using DG.Tweening;
using Game.ECS.System;
using TMPro;
using UniRx;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS_UI.Components
{
    public class UIRegistrationPanel : UIItemControllerBase
    {
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passField;
        [SerializeField] private TMP_InputField nickField;
        [SerializeField] private RegistrationButton nextButton;

        [Space] 
        [SerializeField] private TMP_Text passTitleText;
        [SerializeField] private TMP_Text emailTitleText;
        [SerializeField] private TMP_Text nickTitleText;

        private string saveEmail;
        private string savePass;
        private string saveNick;

        private Tween effect;

        private static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

        private void Awake()
        {
            emailField.ObserveEveryValueChanged(x => x.text).Subscribe(x =>
            {
                saveEmail = x;
            }).AddTo(this);
            
            passField.ObserveEveryValueChanged(x => x.text).Subscribe(x =>
            {
                savePass = x;
            }).AddTo(this);

            nickField.ObserveEveryValueChanged(x => x.text).Subscribe(x =>
            {
                saveNick = x;
            }).AddTo(this);

            nextButton.OnClick.Subscribe(x=> TryRegistration()).AddTo(this);
        }

        private void TryRegistration()
        {
            if (entity != Entity.Null && EntityManager.HasComponent<AsyncTag>(entity)) return;
            
            effect?.Kill();

            nickTitleText.DOColor(new Color(243, 255, 0, 1), 0);
            emailTitleText.DOColor(new Color(243, 255, 0, 1), 0);
            passTitleText.DOColor(new Color(0, 136, 255, 1), 0);

            if (string.IsNullOrEmpty(saveEmail))
            {
                effect = emailTitleText.DOColor(Color.red, .3f);
                
                return;
            }

            if (string.IsNullOrEmpty(savePass))
            {
                effect = passTitleText.DOColor(Color.red, .3f);
                
                return;
            }
            
            if (string.IsNullOrEmpty(saveNick))
            {
                effect = nickTitleText.DOColor(Color.red, .3f);
                
                return;
            }
            
            if (!string.IsNullOrEmpty(saveEmail)
                && !string.IsNullOrEmpty(savePass)
                && !string.IsNullOrEmpty(saveNick))
            {

                if (savePass.Length < 8)
                {
                    effect = passTitleText.DOColor(Color.red, .3f);
                    
                    return;
                }

                if (!IsValidEmail(saveEmail))
                {
                    effect = emailTitleText.DOColor(Color.red, .3f);
                    
                    return;
                }

                if (saveNick.Length <= 0 || saveNick.Length > 10)
                {
                    effect = nickTitleText.DOColor(Color.red, .3f);
                    
                    return;
                }
                
                entity = EntityManager.CreateEntity();

                var data = new FirebaseRegistrationData()
                {
                    email = saveEmail,
                    pass = savePass,
                    nick = saveNick
                };

                EntityManager.AddComponentData(entity, data);
            }
        }

        public void SwitchStateButton(RegistrationButton.State state)
        {
            nextButton.SwitchState(state);
        }
        
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }

    public struct FirebaseRegistrationData : IComponentData
    {
        public FixedString512Bytes email;
        public FixedString512Bytes pass;
        public FixedString512Bytes nick;
    }
}