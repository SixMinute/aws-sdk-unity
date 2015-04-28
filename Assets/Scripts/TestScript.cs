using UnityEngine;

using System;
using System.Collections;
using System.Text;

using Amazon.Unity3D;
using Amazon.CognitoIdentity;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Util;
using Amazon.Runtime.Internal.Util;

public class TestScript : MonoBehaviour
{
	public string IDENTITY_POOL_ID;

	public static readonly RegionEndpoint COGNITO_REGION = RegionEndpoint.USEast1;

	// Use this for initialization
	void Start () {
		AmazonLogging.Level = AmazonLogging.LoggingLevel.DEBUG;
		var credentials = new CognitoAWSCredentials(IDENTITY_POOL_ID, COGNITO_REGION);
		var lambdaClient = new AmazonLambdaClient(credentials, COGNITO_REGION);
		InvokeLambda( lambdaClient, "myHelloWorld", "{\"key1\": 123}", ( result => Debug.Log(result) ) );
	}
	
	public void InvokeLambda(AmazonLambdaClient lambdaClient, string functionName, string payload, Action<string> callback)
	{
		InvokeRequest ir = new InvokeRequest {
			FunctionName = functionName,
			InvocationType = InvocationType.RequestResponse,
			PayloadStream = AWSSDKUtils.GenerateMemoryStreamFromString(payload)
		};
		
		lambdaClient.Invoke(
			ir,
			(result => {
					if(result.Exception != null)
					{
						Debug.LogException(result.Exception);
						return;
					}
					
					callback.Invoke( ASCIIEncoding.ASCII.GetString( ( (InvokeResponse)result.Response ).Payload.ToArray() ) );
				}
			),
			null
		);
		
	}
}
