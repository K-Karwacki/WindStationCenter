import { selectedTurbineAtom } from "@core/atoms/telemetryAtoms";
import EnvironmentSection from "@ui/pages/dashboard_sections/EnvironmentSection";
import { useAtom } from "jotai";
import { useEffect, useState } from "react";
import { type StationClient, TurbineDto } from "src/generated-ts-client";

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

    const selectTurbine = (event: React.MouseEvent<HTMLButtonElement>) => {
        const turbineId = event.currentTarget.value;
        console.log("Selected turbine ID:", turbineId);
        setSelectedTurbine(turbineId);
    };

    return (
        <div>
            {turbines.map(turbine => (
                <button onClick={selectTurbine} key={turbine.turbineExternalId} value={turbine.turbineExternalId}>{turbine.turbineExternalId}</button>
            ))}
            <EnvironmentSection />
        </div>
    );
}