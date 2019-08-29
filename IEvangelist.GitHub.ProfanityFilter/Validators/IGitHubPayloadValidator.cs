namespace IEvangelist.GitHub.Webhooks.Validators
{
    public interface IGitHubPayloadValidator
    {
        bool IsPayloadSignatureValid(byte[] bytes, string receivedSignature);
    }
}