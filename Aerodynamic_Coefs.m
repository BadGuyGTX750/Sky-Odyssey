clear all
close all
clc

e = 2.718;
alfa_graph = -180:1:180;
alfa = (-180:1:180) * pi/180;
Clalfa = 2*pi;
Cd0 = 0.02;
alfa_0_base = 0 * pi/180;
alfa_stall_P_base = 15 * pi/180;
alfa_stall_N_base = -15 * pi/180;
cf_per_c = 0.428;
AR = 5.75;
deflection = 0 * pi/180;
deflection_max = 50 * pi/180;

CLalfa = Clalfa*(AR/(AR+2*(AR+2)/(AR+4)));

teta_f = acos(2 * cf_per_c - 1);
tau = 1 - (teta_f - sin(teta_f))/pi;
eta = 0.4;
delta_CL = CLalfa * tau * eta * deflection;

alfa_0 = alfa_0_base - delta_CL/CLalfa;

delta_CL_max = CLalfa * tau * eta * deflection_max;
CL_maxP = CLalfa * (alfa_stall_P_base - alfa_0_base) + delta_CL * cf_per_c;
CL_maxN = CLalfa * (alfa_stall_N_base - alfa_0_base) + delta_CL * cf_per_c;

alfa_stall_P = alfa_0 + CL_maxP/CLalfa;
alfa_stall_N = alfa_0 + CL_maxN/CLalfa;

alfa_i = zeros(1, 361);
alfa_eff = zeros(1, 361);
CL = zeros(1, 361);
CD= zeros(1, 361);
CM = zeros(1, 361);
CT = zeros(1, 361);
CN = zeros(1, 361);

cnt = 0;

for i = 1:361
    if (alfa_stall_N <= alfa(i) && alfa(i) <= alfa_stall_P)
        CL(i) = CLalfa * (alfa(i) - alfa_0);
        alfa_i(i) = CL(i)/(pi * AR);
        alfa_eff(i) = alfa(i) - alfa_0 - alfa_i(i);
        CT(i) = Cd0 * cos(alfa_eff(i));
        CN(i) = (CL(i) + CT(i) * sin(alfa_eff(i))) / cos(alfa_eff(i));
        CD(i) = CN(i) * sin(alfa_eff(i)) + CT(i) * cos(alfa_eff(i));
        CM(i) = -CN(i) * (0.25 - 0.175 * (1 - 2 * alfa_eff(i)/pi));
    else
        Cd90 = -4.26 * 10^-2 * deflection^2 + 2.1 * 10^-1 * deflection + 1.98;

        if alfa(i) > alfa_stall_P
            tmp = CLalfa * (alfa_stall_P - alfa_0);
        else
            tmp = CLalfa * (alfa_stall_N - alfa_0);
        end

        alfa_i(i) = tmp/(pi * AR);

        if alfa(i) > alfa_stall_P
            lerp = (pi/2 - bound(alfa(i),-pi/2,pi/2))/(pi/2 - alfa_stall_P);
        else
            lerp = (-pi/2 - bound(alfa(i),-pi/2,pi/2))/(-pi/2 - alfa_stall_N);
        end
        alfa_i(i) = lerp * alfa_i(i);

        alfa_eff(i) = alfa(i) - alfa_0 - alfa_i(i);
        CN(i) = Cd90 * sin(alfa_eff(i))*(1/(0.56 + 0.44 * abs(sin(alfa_eff(i)))) - 0.41 * (1 - e^(-17/AR)));
        CT(i) = 0.5 * Cd0 * cos(alfa_eff(i));
        CL(i) = CN(i) * cos(alfa_eff(i)) - CT(i) * sin(alfa_eff(i));
        CD(i) = CN(i) * sin(alfa_eff(i)) + CT(i) * cos(alfa_eff(i));
        CM(i) =  -CN(i) * (0.25 - 0.175 * (1 - 2*abs(alfa_eff(i))/pi));
        cnt = cnt + 1;
    end
end

cnt

figure
plot(alfa_graph,CL,'blue');
figure 
plot(alfa_graph,CD,'red');
figure
plot(alfa_graph,CM,'yellow');





% CL = CLalfa * (alfa - alfa_0);
% alfa_i = CL/(pi * AR);
% alfa_eff = alfa - alfa_0 - alfa_i;
% CT = Cd0 * cos(alfa_eff);
% CN = (CL + CT .* sin(alfa_eff)) ./ cos(alfa_eff);
% CD = CN .* sin(alfa_eff) + CT .* cos(alfa_eff);
% CM = -CN .* (0.25 - 0.175 * (1 - 2 * alfa_eff/pi));
% 
% Cd90 = -4.26 * 10^-2 * deflection^2 + 2.1 * 10^-1 * deflection + 1.98;
% CN = Cd90 * sin(alfa_eff).*(1./(0.56 + 0.44 * sin(alfa_eff)) - 0.41 * (1 - e^(-17/AR)));
% CT = 0.5 * Cd0 * cos(alfa_eff);
% CL = CN .* cos(alfa_eff) - CT .* sin(alfa_eff);
% CD = CN .* sin(alfa_eff) + CT .* cos(alfa_eff);
% CM =  -CN .* (0.25 - 0.175 * (1 - 2*alfa_eff/pi));






