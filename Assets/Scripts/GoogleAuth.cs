// using Firebase;
// using Firebase.Auth;
// using UnityEngine;
// using UnityEngine.UI;

// public class PhoneAuthUI : MonoBehaviour
// {
//     public InputField phoneNumberInputField;
//     public InputField verificationCodeInputField;
//     public Button sendVerificationCodeButton;
//     public Button verifyCodeButton;

//     private FirebaseAuth auth;
//     private string verificationId;

//     void Start()
//     {
//         auth = FirebaseAuth.DefaultInstance;
//         sendVerificationCodeButton.onClick.AddListener(SendVerificationCode);
//         verifyCodeButton.onClick.AddListener(VerifyCode);
//     }

//     public void SendVerificationCode()
//     {
//         string phoneNumber = phoneNumberInputField.text;
//         PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(auth);
//         provider.VerifyPhoneNumber(phoneNumber, TimeSpan.FromSeconds(60), null,
//             verificationCompleted: (credential) =>
//             {
//                 auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
//                 {
//                     if (task.IsFaulted)
//                     {
//                         Debug.LogError("Failed to sign in with phone number: " + task.Exception);
//                     }
//                     else if (task.IsCompleted)
//                     {
//                         Debug.Log("Phone number signed in successfully!");
//                     }
//                 });
//             },
//             verificationFailed: (error) =>
//             {
//                 Debug.LogError("Failed to verify phone number: " + error);
//             },
//             codeSent: (id, token) =>
//             {
//                 PlayerPrefs.SetString("VerificationId", id);
//                 Debug.Log("Verification code sent successfully!");
//             },
//             codeAutoRetrievalTimeOut: (id) =>
//             {
//                 Debug.LogWarning("Verification code retrieval timed out!");
//             });
//     }

//     public void VerifyCode()
//     {
//         string code = verificationCodeInputField.text;
//         string verificationId = PlayerPrefs.GetString("VerificationId");
//         PhoneAuthCredential credential = PhoneAuthProvider.GetCredential(verificationId, code);

//         auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
//         {
//             if (task.IsFaulted)
//             {
//                 Debug.LogError("Failed to sign in with verification code: " + task.Exception);
//             }
//             else if (task.IsCompleted)
//             {
//                 Debug.Log("Verification code signed in successfully!");
//             }
//         });
//     }
// }
