    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using Firebase;
    using Firebase.Firestore;
    using Firebase.Auth;
    using Firebase.Extensions;
    using TMPro;
    using UnityEngine.UI;

    public class FireBase : MonoBehaviour
    {
        
        private FirebaseAuth auth;
        private bool isInitialized = false;
        private string statusMessage = "";
        private bool isLoggedIn = false;

        [SerializeField] private TMP_InputField inputLoginEmail; // TMP_InputField로 변경
        [SerializeField] private TMP_InputField inputLoginPassword; // TMP_InputField로 변경
        [SerializeField] private TMP_InputField inputSignUpEmail; // TMP_InputField로 변경
        [SerializeField] private TMP_InputField inputSignUpPassword; // TMP_InputField로 변경
        [SerializeField] private TMP_InputField inputSignUpnickname; // TMP_InputField로 변경
        
        [SerializeField] private Button buttonStart;
        [SerializeField] private Button buttonSignUp;

        [SerializeField] private LoginButton loginButton;
        
        [SerializeField] private TMP_Text NotificationText; 
        [SerializeField] private SavePlayerBasicStat savePlayerBasicStat;
        
        private Dictionary<string, FirebaseAuth> playerAuths = new Dictionary<string, FirebaseAuth>();
        private Dictionary<string, FirebaseFirestore> playerFirestore = new Dictionary<string, FirebaseFirestore>();
        
        private void Start()
        {
            buttonStart.interactable = false;
            buttonSignUp.interactable = false;
            InitFirebase();
            SetInputSound();
        }

        private void SetInputSound()
        {
            inputLoginEmail.onValueChanged.AddListener((_) => SoundManager.Instance.PlaySound(SoundType.Input));
            inputLoginPassword.onValueChanged.AddListener((_) => SoundManager.Instance.PlaySound(SoundType.Input));
            inputSignUpEmail.onValueChanged.AddListener((_) => SoundManager.Instance.PlaySound(SoundType.Input));
            inputSignUpPassword.onValueChanged.AddListener((_) => SoundManager.Instance.PlaySound(SoundType.Input));
            inputSignUpnickname.onValueChanged.AddListener((_) => SoundManager.Instance.PlaySound(SoundType.Input));
        }

        private void InitFirebase()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    FirebaseApp app = FirebaseApp.DefaultInstance;
                    auth = FirebaseAuth.DefaultInstance;
                    isInitialized = true;
                    statusMessage = "Firebase 초기화 성공";
                    Debug.Log(statusMessage);
            
                    // 초기화가 완료되면 버튼 활성화
                    buttonStart.interactable = true;
                    buttonSignUp.interactable = true;
                }
                else
                {
                    statusMessage = "Firebase 초기화 실패";
                    Debug.Log(statusMessage);
                }
            });
        }

        public void OnStartButtonClicked()
        {
            SoundManager.Instance.PlaySound(SoundType.Button);
            
            string email = inputLoginEmail.text;
            string password = inputLoginPassword.text;

            SignIn(email, password);
        }

        public void OnSignUpButtonClicked()
        {
            SoundManager.Instance.PlaySound(SoundType.Button);

            string email = inputSignUpEmail.text;
            string password = inputSignUpPassword.text;
            string nickname = inputSignUpnickname.text;
            CreateAccount(email, password, nickname);
        }

        private void CreateAccount(string email, string password, string nickname)
        {
            try
            {
                if (!isInitialized)
                {
                    Debug.LogError("Firebase가 초기화되지 않았습니다.");
                    return;
                }

                // 고유한 FirebaseApp 생성 (플레이어마다)
                FirebaseApp playerApp = FirebaseApp.Create(new AppOptions
                {
                    ProjectId = FirebaseApp.DefaultInstance.Options.ProjectId,
                    ApiKey = FirebaseApp.DefaultInstance.Options.ApiKey,
                    AppId = FirebaseApp.DefaultInstance.Options.AppId
                }, email);

                // 각 플레이어의 FirebaseAuth와 Firestore 인스턴스 독립적 관리
                FirebaseAuth playerAuth = FirebaseAuth.GetAuth(playerApp);
                FirebaseFirestore firestore = FirebaseFirestore.GetInstance(playerApp);

                // 플레이어 별로 FirebaseAuth, Firestore 저장
                playerAuths[email] = playerAuth;
                playerFirestore[email] = firestore;

                playerAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                        FirebaseUser newUser = task.Result.User;
                        statusMessage = "회원가입 성공";

                        SaveUserToFirestore(newUser.UserId, email, HashPassword(password), nickname, email);
                    }
                    else
                    {
                        statusMessage = "회원가입 실패: " + task.Exception?.Message;
                        NotificationText.gameObject.SetActive(true);
                        NotificationText.text = "already ID exist";
                        Debug.Log(statusMessage);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError("CreateAccount에서 예외 발생: " + ex.Message);
            }
        }

        private void SaveUserToFirestore(string userId, string email, string hashedPassword, string nickname, string playerEmail)
        {
            if (!playerFirestore.ContainsKey(playerEmail))
            {
                Debug.LogError("Firestore 인스턴스를 찾을 수 없습니다.");
                return;
            }

            FirebaseFirestore db = playerFirestore[playerEmail];
            DocumentReference userDocRef = db.Collection("users").Document(userId);

            Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "email", email },
                { "password", hashedPassword },
                { "nickname", nickname }
            };

            userDocRef.SetAsync(userData).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    Debug.Log("<color=green>Firestore에 사용자 정보 저장 성공</color>");
                }
                else
                {
                    Debug.Log("<color=red>Firestore에 사용자 정보 저장 실패: " + task.Exception?.Message + "</color>");
                }
            });
        }

        private string HashPassword(string password)
        {
            var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }



private void SignIn(string email, string password)
{
    try
    {
        if (!isInitialized)
        {
            Debug.LogError("Firebase가 초기화되지 않았습니다.");
            return;
        }

        // 고유한 FirebaseApp 생성 (플레이어마다)
        FirebaseApp playerApp = FirebaseApp.Create(new AppOptions
        {
            ProjectId = FirebaseApp.DefaultInstance.Options.ProjectId,
            ApiKey = FirebaseApp.DefaultInstance.Options.ApiKey,
            AppId = FirebaseApp.DefaultInstance.Options.AppId
        }, email);

        // 각 플레이어의 FirebaseAuth와 Firestore 인스턴스 독립적 관리
        FirebaseAuth playerAuth = FirebaseAuth.GetAuth(playerApp);
        FirebaseFirestore firestore = FirebaseFirestore.GetInstance(playerApp);
        
        playerAuths[email] = playerAuth;
        playerFirestore[email] = firestore;

        playerAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            
            if (task.IsFaulted)
            {
                if (task.Exception != null && task.Exception.InnerException is FirebaseException firebaseEx)
                {
                    var errorCode = ((FirebaseException)firebaseEx).ErrorCode;
                    
                    // 이메일이 없을 때
                    if (errorCode == (int)AuthError.UserNotFound)
                    {
                        NotificationText.gameObject.SetActive(true);
                        NotificationText.text = "Email not found";
                    }
                    // 비밀번호가 틀렸을 때
                    else if (errorCode == (int)AuthError.WrongPassword)
                    {
                        NotificationText.gameObject.SetActive(true);
                        NotificationText.text = "Password is incorrect";
                    }
                    else
                    {
                        NotificationText.gameObject.SetActive(true);
                        NotificationText.text = "ex";
                        Debug.LogError("로그인 중 예외 발생: " + firebaseEx.Message);
                    }
                }
                else
                {
                    Debug.LogError("로그인 중 알 수 없는 예외 발생: " + task.Exception?.Message);
                }
                return;
            }

            if (task.IsCompleted)
            {
                
                FirebaseUser newUser = task.Result.User;
                
                statusMessage = "로그인 성공";
                isLoggedIn = true;
                Debug.Log(statusMessage);
                
                LoadUserEmailAndPasswordFromFirestore(newUser.UserId, email);
                loginButton.OnStartButton();


            }
                
        });
    }
    catch (Exception ex)
    {
        Debug.LogError("SignIn에서 예외 발생: " + ex.Message);
    }
}

// Firestore에서 사용자 이메일과 비밀번호 불러오기 (사용자별 Firestore 사용)
private void LoadUserEmailAndPasswordFromFirestore(string userId, string email)
{
    if (!playerFirestore.ContainsKey(email))
    {
        Debug.LogError("Firestore 인스턴스를 찾을 수 없습니다.");
        return;
    }

    FirebaseFirestore db = playerFirestore[email];
    DocumentReference userDocRef = db.Collection("users").Document(userId);

    userDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Firestore에서 사용자 정보 로드 실패: " + task.Exception?.Message);
            return;
        }

        if (task.IsCompleted)
        {
            var snapshot = task.Result;
            if (snapshot.Exists)
            {
                string userEmail = snapshot.GetValue<string>("email");
                string userPassword = snapshot.ContainsField("password") ? snapshot.GetValue<string>("password") : "비밀번호 없음";
                string userNickname = snapshot.GetValue<string>("nickname");

                Debug.Log($"사용자 이메일: {userEmail}");
                Debug.Log($"사용자 비밀번호 (해시): {userPassword}");
                Debug.Log($"사용자 닉네임: {userNickname}");

                savePlayerBasicStat.Email = userEmail;
                savePlayerBasicStat.Password = userPassword;
                savePlayerBasicStat.Nickname = userNickname;
            }
            else
            {
                Debug.LogWarning("Firestore에서 사용자 정보를 찾을 수 없습니다.");
            }
        }
    });
}
    // Firestore에서 사용자 이메일과 비밀번호 불러오기
    private void LoadUserEmailAndPasswordFromFirestore(string userId)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userDocRef = db.Collection("users").Document(userId);

        userDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firestore에서 사용자 정보 로드 실패: " + task.Exception?.Message);
                return;
            }

            if (task.IsCompleted)
            {
                var snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string userEmail = snapshot.GetValue<string>("email");
                    string userPassword = snapshot.ContainsField("password") ? snapshot.GetValue<string>("password") : "비밀번호 없음";
                    string userNickname = snapshot.GetValue<string>("nickname");

                    Debug.Log($"사용자 이메일: {userEmail}");
                    Debug.Log($"사용자 비밀번호 (해시): {userPassword}");
                    Debug.Log($"사용자 닉네임: {userNickname}");
                }
                else
                {
                    Debug.LogWarning("Firestore에서 사용자 정보를 찾을 수 없습니다.");
                }
            }
        });
    } 
    }