using UnityEngine;
using brainflow;

public class SimpleGetData : MonoBehaviour
{
    private BoardShim boardShim;
    private int samplingRate;
    
    private void Start()
    {
        try
        {
            BoardShim.set_log_file("brainflow_log.txt");
            BoardShim.enable_dev_board_logger();

            var inputParams = new BrainFlowInputParams();
            const int boardId = (int)BoardIds.SYNTHETIC_BOARD;
            boardShim = new BoardShim(boardId, inputParams);
            boardShim.prepare_session();
            boardShim.start_stream(450000, "file://brainflow_data.csv:w");
            samplingRate = BoardShim.get_sampling_rate(boardId);
            Debug.Log("Brainflow streaming was started");
        }
        catch (BrainFlowError e)
        {
            Debug.Log(e);
        }
    }
    
    private void Update()
    {
        if (boardShim == null)
        {
            return;
        }
        var numberOfDataPoints = samplingRate * 4;
        var data = boardShim.get_current_board_data(numberOfDataPoints);
        // Check https://brainflow.readthedocs.io/en/stable/index.html for API reference and more code samples
        Debug.Log("Num elements: " + data.GetLength(1));
    }

    // You need to call release_session and ensure that all resources correctly released
    private void OnDestroy()
    {
        if (boardShim == null) return;
        try
        {
            boardShim.release_session();
        }
        catch (BrainFlowError e)
        {
            Debug.Log(e);
        }
        Debug.Log("Brainflow streaming was stopped");
    }
}