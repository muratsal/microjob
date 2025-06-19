USE [ERP]
GO

/****** Object:  StoredProcedure [dbo].[GetNextCounterValue]    Script Date: 20.10.2021 11:48:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[GetNextCounterValue]
(
	@CounterName nvarchar(50)
)
As
Begin
	
	Declare @CurrentValue int
	Declare @AddYear bit
	Declare @PaddingCount int
	Declare @Prefix nvarchar(50)
	Declare @FormattedCounter nvarchar(50)

    Select  @CurrentValue = CurrentValue,
			@AddYear = AddYear,
			@PaddingCount = PaddingCount,
			@Prefix = Prefix
	From Counters 
	Where CounterName = @CounterName
	
	Select @CurrentValue = @CurrentValue + 1
	
	Update Counters Set CurrentValue = @CurrentValue  Where CounterName = @CounterName

	Select @FormattedCounter = @Prefix 

	if(@AddYear=1)
	Begin
		Select @FormattedCounter = @Prefix + RIGHT(CAST(YEAR(GETDATE()) as nvarchar(4)),2)
	End

	Select @FormattedCounter = @FormattedCounter +  RIGHT('0000000000'+CAST(@CurrentValue AS VARCHAR(10)),@PaddingCount)

	Select  @FormattedCounter CounterValue
 
End
GO


