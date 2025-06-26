namespace Application.DTO.Requests.General;

public interface IPaginationRequest
{
    public int Take { get; }
    public int Skip { get; }
}