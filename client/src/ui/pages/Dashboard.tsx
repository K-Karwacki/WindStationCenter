import { selectedTurbineAtom } from "@core/atoms/telemetryAtoms";
import EnvironmentSection from "@ui/pages/dashboard_sections/EnvironmentSection";
import { useAtom } from "jotai";
import { useEffect, useState } from "react";
import { type StationClient, TurbineDto } from "src/generated-ts-client";
import MechanicalSection from "./dashboard_sections/MechanicalSection";
import ThermalSection from "./dashboard_sections/ThermalSection";
import TurbineControls from '@ui/components/TurbineControls';

export function Dashboard(props: { stationControllerClient: StationClient }) {
    const [turbines, setTurbines] = useState<TurbineDto[]>([]);
    const [selectedTurbine, setSelectedTurbine] = useAtom(selectedTurbineAtom);

    const { stationControllerClient } = props;
    useEffect(() => {
        const fetchTurbines = async () => {
            try {
                const data = await stationControllerClient.getTurbines() as TurbineDto[];
                setTurbines(data);
                console.log("Fetched turbines data:", data);
            } catch (error) {
                console.error("Error fetching turbines data:", error);
            }
        };

        fetchTurbines();
    }, []);

    // simple toggle: click selects, click again deselects
    const toggleSelect = (id: string) => {
        setSelectedTurbine(prev => (prev === id ? null : id));
    };

    return (
        <div className="w-[100rem]">
                        <div className="flex gap-2 flex-wrap mb-4">
                            {turbines.map(turbine => {
                                const id = turbine.turbineExternalId as string;
                                const isSelected = id === selectedTurbine;
                                const base = "px-3 py-1 rounded-md border! transition-colors";
                                const selectedCls = "btn-active";
                                const normalCls = "btn-ghost";
                                return (
                                    <button
                                        type="button"
                                        onClick={() => toggleSelect(id)}
                                        key={id}
                                        aria-pressed={isSelected}
                                        className={`${base} ${isSelected ? selectedCls : normalCls} w-[10rem] h-[3rem] text-lg font-medium flex items-center justify-center`}
                                    >
                                        {id}
                                    </button>
                                );
                            })}
                        </div>
            <TurbineControls />
            <EnvironmentSection />
            <MechanicalSection />
            <ThermalSection />
        </div>
    );
}