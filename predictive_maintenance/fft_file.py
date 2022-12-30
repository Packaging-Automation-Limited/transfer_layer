import adi
import numpy
import time
import matplotlib.pyplot as plt

plt.ioff()

uri = "ip:10.0.0.106" #10.10.0.128
sample_freq = 128000 #256000
xl = adi.cn0532(uri)
print("SW_CC: ",xl.sw_cc)
print("Sample Rate: ",xl.sample_rate)
print("ctx: ",xl.ctx)
print("fda_disable_status: ",xl.fda_disable_status)
print("fda_mode: ",xl.fda_mode)
print("input_voltage: ",xl.input_voltage)
print("red_led_enable: ",xl.red_led_enable)
print("rx_enabled_channels: ",xl.rx_enabled_channels)
enblCh = xl.rx_enabled_channels
print("sensor_voltage: ",xl.sensor_voltage)
print("monitor_powerup: ", xl.monitor_powerup)


#xl.ctx.set_timeout(100000)
xl.monitor_powerup = 1
xl.fda_disable_status = 0
xl.fda_mode = "full-power"
xl.sample_rate = sample_freq
xl.sw_cc = 1
time.sleep(3)
xl.calibrate()
time.sleep(5)
print("SW_CC: ",xl.sw_cc)
print("Sample Rate: ",xl.sample_rate)


for x in range(6):
    input("Press Enter to continue...")
    #time.sleep(0.5)
    xl = adi.cn0532(uri)
    # Set number of samples to capture
    xl.rx_buffer_size = 2**17 # 2**17
    # Pull rx_buffer_size samples back from the device
    data = xl.rx()
    

    numpy.savetxt(".\Data\data.csv", data, delimiter=",")

    tpCount = len(data)
    values = numpy.arange(int(tpCount/2))
    timePeriod = tpCount / sample_freq
    frequencies = values / timePeriod

    fftnew = numpy.fft.fft(data)/tpCount # Normalize amplitude
    fftnew = fftnew[range(int(tpCount/2))] # Exclude sampling frequency

    #plt.plot(frequencies, abs(fftnew))
    #plt.show()

    datafft = numpy.fft.fft(data)
    
    numpy.savetxt(".\Data\datafft.csv", datafft, delimiter=",")

    #n = data.size
    #timestep = 0.1
    #datafftfreq = numpy.fft.fftfreq(n, d=timestep)
    frequencies = numpy.delete(frequencies,[0,1])
    numpy.savetxt(".\Data\datafftfreq.csv", frequencies, delimiter=",")

    #datahfft = numpy.fft.hfft(data)
    fftnew = numpy.delete(fftnew,[0,1])
    numpy.savetxt(".\Data\datahfft.csv", abs(fftnew), delimiter=",")
    fileName = ".\Data\datahfft" + str(6-x) + ".csv"
    print (fileName)
    numpy.savetxt(fileName, abs(fftnew), delimiter=",")
    print("Run: " + str(x))

    #plt.plot(data)
    #plt.show()
    #plt.plot(frequencies, abs(fftnew))
    #plt.show()

    time.sleep(1)
print("End"
