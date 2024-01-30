namespace ControleCarteira.Domain;
public class ObjectMessageProcessedInfo
{
    public Ordem Object { get; set; }
    public bool ProcessedSuccessfully { get; set; }
    public bool ProcessedDlqQueue { get; set; }
}
