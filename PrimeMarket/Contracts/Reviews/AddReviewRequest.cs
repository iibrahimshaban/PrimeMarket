namespace PrimeMarket.Contracts.Reviews;

public record AddReviewRequest(
    int Rating, 
    string? Comment
    );
