param (
    [string]$url,     
    [int]$duration 
    )  
    
"URL: $url"; 
"Duration: $duration seconds`n";  
$starttime = get-date; 
$i=0; 
$transfered = 0;
$webClient = new-object Net.WebClient;  
$webClient.UseDefaultCredentials = $true

while (1) 
{     
    $transfered += $webClient.DownloadString($url).Length;   
    $i++;       
    $timespan = new-timespan $starttime;       
    if ($timespan.TotalSeconds -ge $duration)       
    {            
        "`n`n$i requests for url $url served in $duration seconds."            
        break;       
    }       
    else       
    {            
        if ($i%100 -eq 0)            
        {                 
            $dursec = [int]$timespan.TotalSeconds;                    
            $mb = $transfered / 1MB;
            if ($timespan.Seconds -gt 0) 
            {
                $speed = ($transfered / $timespan.Seconds) / 1MB;
            }
            else
            {
                $speed = "NA";
            }

            write-host -noNewLine "`r$i requests served ($dursec seconds) ($mb MB) ($speed MB/s)";            
        }       
    } 
}
