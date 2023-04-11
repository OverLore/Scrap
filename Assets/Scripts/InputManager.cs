using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    private PlayerInputs m_inputs;
    public PlayerInputs Inputs {
        get {
            if (m_inputs == null)
            {
                m_inputs = new PlayerInputs();
                m_inputs.Enable();
            }

            return m_inputs;
        }
    }

    public void Init()
    {
        m_inputs = new PlayerInputs();
        m_inputs.Enable();
    }
}
