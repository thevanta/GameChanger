%% Initialize
close all; clear all; clc;

%% Load in dataset
%load('data.mat')
data = load('ID1.mat');

%% You can change the settings below for sensor, type, and trial_num
subject = 'ID1';
sensor = 'thighR'; % Options are trunk, thighR, thighL, shankR, shankL, wrist
type = {'CALIB'}; % Options include: {'BnkL';'BnkR';'CALIB';'CS';'FE';'GR';'SlpD';'SlpU';'StrD';'StrU'}; check README.txt for more details
trial_num = 1;

%% Preprocess data
idx = ismember(data.(subject).(sensor).Surface, type);
d = data.(subject).(sensor)(idx, :);

accel = [d.Acc_X{trial_num}, d.Acc_Y{trial_num}, d.Acc_Z{trial_num}];
gyro = [d.Gyr_X{trial_num}, d.Gyr_Y{trial_num}, d.Gyr_Z{trial_num}]; 
mag = [d.Mag_X{trial_num}, d.Mag_Y{trial_num}, d.Mag_Z{trial_num}];
delta_quat_true = [d.OriInc_q0{trial_num}, d.OriInc_q1{trial_num}, d.OriInc_q2{trial_num}, d.OriInc_q3{trial_num}]; %OriInc_q# 4 N/A 3D delta_quaternion (dq); result of SDI algorithm
euler_true = [d.Roll{trial_num}, d.Pitch{trial_num}, d.Yaw{trial_num}];

missing_num = d.MissingCount{trial_num};
missing_pct = d.Missingpct{trial_num};

accel = rmmissing(accel); % remove any NaNs
gyro = rmmissing(gyro); % remove any NaNs
mag = rmmissing(mag); % remove any NaNs
delta_quat_true = rmmissing(delta_quat_true); % remove any NaNs
euler_true = rmmissing(euler_true); % remove any NaNs

quat_true = eul2quat(deg2rad(euler_true), 'ZYX');

%% Fusion using matlab toolbox
Fs = 100;
fuse = ahrsfilter('SampleRate',Fs);
q = fuse(accel, gyro, mag);
quat = compact(q); % output w + _i + _j + _k
euler = eulerd(q , 'ZYX', 'frame'); % these given options output in yay, pitch, roll (partially done to match ground truth)

%% Plot Quaternion outputs
figure

subplot(2,2,1)
plot(quat(:,1));
hold on;
plot(quat_true(:,1));
title('Orientation -- W')
xlabel('Sample')
legend('Computed', 'True')

subplot(2,2,2)
plot(quat(:,2));
hold on;
plot(quat_true(:,4));
title('Orientation -- X')
xlabel('Sample')
legend('Computed', 'True')

subplot(2,2,3)
plot(quat(:,3));
hold on;
plot(quat_true(:,3));
title('Orientation -- Y')
xlabel('Sample')
legend('Computed', 'True')

subplot(2,2,4)
plot(quat(:,4));
hold on;
plot(quat_true(:,2));
title('Orientation -- Z')
xlabel('Sample')
legend('Computed', 'True')

%% Plot Euler Angles
figure

subplot(3,1,1)
plot(euler(:,3));
hold on
plot(euler_true(:,1));
title('Roll')
xlabel('Sample')
legend('Computed', 'True')

subplot(3,1,2)
plot(euler(:,2));
hold on
plot(euler_true(:,2));
title('Pitch')
xlabel('Sample')
legend('Computed', 'True')

subplot(3,1,3)
plot(euler(:,1));
hold on
plot(euler_true(:,3));
title('Yaw')
xlabel('Sample')
legend('Computed', 'True')

%% RMSE metrics
rmse_roll = sqrt(mean(euler(:,3) - euler_true(:,1)).^2);
rmse_pitch = sqrt(mean(euler(:,2) - euler_true(:,2)).^2);
rmse_yaw = sqrt(mean(euler(:,1) - euler_true(:,3)).^2);

rmse_w = sqrt(mean(quat(:,1) - quat_true(:,1)).^2);
rmse_x = sqrt(mean(quat(:,2) - quat_true(:,4)).^2);
rmse_y = sqrt(mean(quat(:,3) - quat_true(:,3)).^2);
rmse_z = sqrt(mean(quat(:,4) - quat_true(:,2)).^2);

rmse_euler_out = [rmse_roll, rmse_pitch, rmse_yaw]
rmse_quat_out = [rmse_w, rmse_x, rmse_y, rmse_z]

%% Integration of Signals
% Ref -- https://www.mathworks.com/matlabcentral/answers/809765-displacement-from-accelerometer-data
acc = detrend(accel);
% Time
tStep = 0.01;      % Length of each time step
N = length(acc)*tStep;
t = 0:tStep:N;
t(end) = [];
N = length(t);
dt = mean(diff(t));       % Average dt
fs = 1/dt;                  % Frequency [Hz] or sampling rate
% some additionnal high pass filtering
N = 2;
fc = 0.5; % Hz
[B,A] = butter(N,2*fc/fs,'high');
acc2 = filter(B,A,acc);
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
vel = cumtrapz(dt,acc2);
vel = detrend(vel);
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
pos     = cumtrapz(dt,vel);

figure
subplot(3,3,1)
plot(acc2(:,1))
title('Acc X')
subplot(3,3,2)
plot(acc2(:,2))
title('Acc Y')
subplot(3,3,3)
plot(acc2(:,3))
title('Acc Z')
subplot(3,3,4)
plot(vel(:,1))
title('Vel X')
subplot(3,3,5)
plot(vel(:,2))
title('Vel Y')
subplot(3,3,6)
plot(vel(:,3))
title('Vel Z')
subplot(3,3,7)
plot(pos(:,1))
title('Pos X')
subplot(3,3,8)
plot(pos(:,2))
title('Pos Y')
subplot(3,3,9)
plot(pos(:,3))
title('Pos Z')

%% Consolidate data

% output w + _i + _j + _k
export_data = array2table([acc2, vel, pos, quat],'VariableNames', {'AccX', 'AccY', 'AccZ', 'VelX', 'VelY', 'VelZ', 'PosX', 'PosY', 'PosZ', 'QuatW', 'QuatI', 'QuatJ', 'QuatK'});
writetable(export_data, [string(sensor) + '_' + string(type) + '_' + string(trial_num) + '.csv']);

