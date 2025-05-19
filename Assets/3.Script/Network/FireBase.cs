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

        [SerializeField] private TMP_InputField inputLoginEmail; 
        [SerializeField] private TMP_InputField inputLoginPassword; 
        [SerializeField] private TMP_InputField inputSignUpEmail; 
        [SerializeField] private TMP_InputField inputSignUpPassword; 
        [SerializeField] private TMP_InputField inputSignUpnickname; 
        
        [SerializeField] private TMP_Text statusText;
        
        [SerializeField] private Button buttonStart;
        [SerializeField] private Button buttonSignUp;

        [SerializeField] public LoginButton loginButton; 
        
        //[SerializeField] private GameObject PlayerPrefab;
        private void Start()
        {
            buttonStart.interactable = false;
            buttonSignUp.interactable = false;
            InitFirebase();
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
            string email = inputLoginEmail.text;
            string password = inputLoginPassword.text;
            Debug.Log("1");
            SignIn(email, password);
            Debug.Log("9");
        }

        private void OnSignUpButtonClicked()
        {
            string email = inputSignUpEmail.text;
            string password = inputSignUpPassword.text;
            string nickname = inputSignUpnickname.text;
            CreateAccount(email, password, nickname);
        }

        private void CreateAccount(string email, string password, string nickname)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    FirebaseUser newUser = task.Result.User;
                    statusMessage = "회원가입 성공";
                    isLoggedIn = true;

                    SaveUserToFirestore(newUser.UserId, email, HashPassword(password),nickname);
                }
                else
                {
                    statusMessage = "회원가입 실패: " + task.Exception?.Message;
                    Debug.Log(statusMessage);
                }
            });
        }

        private void SaveUserToFirestore(string userId, string email, string hashedPassword,string nickname)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
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

        private Dictionary<string, FirebaseAuth> playerAuths = new Dictionary<string, FirebaseAuth>();
private Dictionary<string, FirebaseFirestore> playerFirestore = new Dictionary<string, FirebaseFirestore>();

private void SignIn(string email, string password)
{
    Debug.Log("2");
    try
    {
        if (!isInitialized)
        {
            Debug.LogError("Firebase가 초기화되지 않았습니다.");
            statusText.text = "Firebase가 초기화되지 않았습니다.";
            return;
        }

        if (auth.CurrentUser != null)
        {
            Debug.LogWarning("이미 로그인된 상태입니다.");
            statusText.text = "ID is login";
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            Debug.Log("3");
            if (task.IsFaulted)
            {
                Debug.LogError("로그인 중 예외 발생: " + task.Exception?.Message);
                statusText.text = "로그인 실패: " + task.Exception?.Message;
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log("4");
                FirebaseUser newUser = task.Result.User;
                statusMessage = "로그인 성공";
                isLoggedIn = true;
                statusText.text = statusMessage;

                // 로그인 성공 시 LoginButton의 OnStartButton 호출
                if (loginButton != null)
                {
                    loginButton.OnStartButton();
                }
                else
                {
                    Debug.LogError("LoginButton 스크립트가 연결되지 않았습니다.");
                }
            }
        });
    }
    catch (Exception ex)
    {
        Debug.LogError("SignIn에서 예외 발생: " + ex.Message);
        statusText.text = "SignIn에서 예외 발생: " + ex.Message;
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